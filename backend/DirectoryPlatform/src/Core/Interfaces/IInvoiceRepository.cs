using DirectoryPlatform.Core.Entities.Bookkeeping;

namespace DirectoryPlatform.Core.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetWithDetailsAsync(Guid id);
    Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId);
    Task<(IEnumerable<Invoice> Items, int TotalCount)> SearchAsync(Guid? userId, string? status, DateTime? from, DateTime? to, int page, int pageSize);
    Task<string> GetNextInvoiceNumberAsync();
    Task<decimal> GetTotalRevenueAsync(DateTime? from, DateTime? to);
    Task<int> GetCountByStatusAsync(string status);
}
