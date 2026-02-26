using DirectoryPlatform.Core.Entities.Bookkeeping;

namespace DirectoryPlatform.Core.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId);
}
