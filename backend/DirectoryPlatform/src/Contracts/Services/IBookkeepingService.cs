using DirectoryPlatform.Contracts.DTOs.Bookkeeping;
using DirectoryPlatform.Contracts.DTOs.Common;

namespace DirectoryPlatform.Contracts.Services;

public interface IBookkeepingService
{
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request);
    Task<InvoiceDto?> GetInvoiceAsync(Guid invoiceId);
    Task<PagedResultDto<InvoiceSummaryDto>> SearchInvoicesAsync(InvoiceSearchRequest request);
    Task<InvoiceDto> UpdateInvoiceStatusAsync(Guid invoiceId, UpdateInvoiceStatusRequest request);
    Task<PaymentDto> RecordPaymentAsync(CreatePaymentRequest request, Guid recordedByUserId);
    Task<BookkeepingReportDto> GetReportAsync(DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<InvoiceDto>> GetUserInvoicesAsync(Guid userId);
}
