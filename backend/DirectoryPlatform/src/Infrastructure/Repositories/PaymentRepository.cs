using DirectoryPlatform.Core.Entities.Bookkeeping;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId)
        => await _dbSet.Where(p => p.InvoiceId == invoiceId).OrderByDescending(p => p.CreatedAt).ToListAsync();
}
