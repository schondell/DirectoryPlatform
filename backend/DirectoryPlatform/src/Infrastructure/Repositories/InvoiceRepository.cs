using DirectoryPlatform.Core.Entities.Bookkeeping;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Invoice?> GetWithDetailsAsync(Guid id)
        => await _dbSet.Include(i => i.User).Include(i => i.LineItems.OrderBy(li => li.DisplayOrder)).Include(i => i.Payments).FirstOrDefaultAsync(i => i.Id == id);

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
        => await _dbSet.Include(i => i.LineItems).Where(i => i.UserId == userId).OrderByDescending(i => i.IssueDate).ToListAsync();

    public async Task<(IEnumerable<Invoice> Items, int TotalCount)> SearchAsync(Guid? userId, string? status, DateTime? from, DateTime? to, int page, int pageSize)
    {
        var query = _dbSet.Include(i => i.User).AsQueryable();

        if (userId.HasValue) query = query.Where(i => i.UserId == userId.Value);
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<InvoiceStatus>(status, true, out var s)) query = query.Where(i => i.Status == s);
        if (from.HasValue) query = query.Where(i => i.IssueDate >= from.Value);
        if (to.HasValue) query = query.Where(i => i.IssueDate <= to.Value);

        var total = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.IssueDate).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<string> GetNextInvoiceNumberAsync()
    {
        var year = DateTime.UtcNow.Year;
        var prefix = $"INV-{year}-";
        var lastInvoice = await _dbSet.Where(i => i.InvoiceNumber.StartsWith(prefix)).OrderByDescending(i => i.InvoiceNumber).FirstOrDefaultAsync();

        if (lastInvoice == null) return $"{prefix}0001";

        var lastNumber = int.Parse(lastInvoice.InvoiceNumber[(prefix.Length)..]);
        return $"{prefix}{(lastNumber + 1):D4}";
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime? from, DateTime? to)
    {
        var query = _dbSet.Where(i => i.Status == InvoiceStatus.Paid);
        if (from.HasValue) query = query.Where(i => i.PaidDate >= from.Value);
        if (to.HasValue) query = query.Where(i => i.PaidDate <= to.Value);
        return await query.SumAsync(i => i.TotalAmount);
    }

    public async Task<int> GetCountByStatusAsync(string status)
    {
        if (!Enum.TryParse<InvoiceStatus>(status, true, out var s)) return 0;
        return await _dbSet.CountAsync(i => i.Status == s);
    }
}
