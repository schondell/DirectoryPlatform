using DirectoryPlatform.Contracts.DTOs.Bookkeeping;
using DirectoryPlatform.Contracts.DTOs.Common;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities.Bookkeeping;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class BookkeepingService : IBookkeepingService
{
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IPaymentRepository _paymentRepo;
    private readonly IUnitOfWork _unitOfWork;

    public BookkeepingService(IInvoiceRepository invoiceRepo, IPaymentRepository paymentRepo, IUnitOfWork unitOfWork)
    {
        _invoiceRepo = invoiceRepo;
        _paymentRepo = paymentRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        var invoiceNumber = await _invoiceRepo.GetNextInvoiceNumberAsync();

        var lineItems = request.LineItems.Select((li, i) => new InvoiceLineItem
        {
            Id = Guid.NewGuid(),
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            TotalPrice = li.Quantity * li.UnitPrice,
            DisplayOrder = i + 1
        }).ToList();

        var subtotal = lineItems.Sum(li => li.TotalPrice);
        var taxAmount = subtotal * 0.081m; // Swiss VAT 8.1%

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = invoiceNumber,
            UserId = request.UserId,
            ListingId = request.ListingId,
            SubscriptionId = request.SubscriptionId,
            Subtotal = subtotal,
            TaxAmount = Math.Round(taxAmount, 2),
            TotalAmount = Math.Round(subtotal + taxAmount, 2),
            DueDate = request.DueDate,
            Notes = request.Notes,
            LineItems = lineItems
        };

        await _invoiceRepo.AddAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return await GetInvoiceAsync(invoice.Id) ?? throw new InvalidOperationException("Failed to retrieve created invoice");
    }

    public async Task<InvoiceDto?> GetInvoiceAsync(Guid invoiceId)
    {
        var invoice = await _invoiceRepo.GetWithDetailsAsync(invoiceId);
        return invoice == null ? null : MapToDto(invoice);
    }

    public async Task<PagedResultDto<InvoiceSummaryDto>> SearchInvoicesAsync(InvoiceSearchRequest request)
    {
        var (items, total) = await _invoiceRepo.SearchAsync(request.UserId, request.Status, request.FromDate, request.ToDate, request.PageNumber, request.PageSize);

        return new PagedResultDto<InvoiceSummaryDto>
        {
            Items = items.Select(i => new InvoiceSummaryDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                UserName = i.User?.Username,
                TotalAmount = i.TotalAmount,
                Currency = i.Currency,
                Status = i.Status.ToString(),
                IssueDate = i.IssueDate,
                DueDate = i.DueDate
            }),
            TotalCount = total,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public async Task<InvoiceDto> UpdateInvoiceStatusAsync(Guid invoiceId, UpdateInvoiceStatusRequest request)
    {
        var invoice = await _invoiceRepo.GetWithDetailsAsync(invoiceId) ?? throw new KeyNotFoundException("Invoice not found");

        if (Enum.TryParse<InvoiceStatus>(request.Status, true, out var status))
        {
            invoice.Status = status;
            if (status == InvoiceStatus.Paid) invoice.PaidDate = DateTime.UtcNow;
        }

        if (!string.IsNullOrEmpty(request.Notes))
            invoice.Notes = (invoice.Notes ?? "") + "\n" + request.Notes;

        await _invoiceRepo.UpdateAsync(invoice);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(invoice);
    }

    public async Task<PaymentDto> RecordPaymentAsync(CreatePaymentRequest request, Guid recordedByUserId)
    {
        var invoice = await _invoiceRepo.GetWithDetailsAsync(request.InvoiceId) ?? throw new KeyNotFoundException("Invoice not found");

        if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, true, out var method))
            throw new ArgumentException($"Invalid payment method: {request.PaymentMethod}");

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            InvoiceId = request.InvoiceId,
            Amount = request.Amount,
            PaymentMethod = method,
            Status = PaymentStatus.Completed,
            TransactionReference = request.TransactionReference,
            Notes = request.Notes,
            ProcessedAt = DateTime.UtcNow
        };

        await _paymentRepo.AddAsync(payment);

        var totalPaid = invoice.Payments.Sum(p => p.Amount) + payment.Amount;
        if (totalPaid >= invoice.TotalAmount)
        {
            invoice.Status = InvoiceStatus.Paid;
            invoice.PaidDate = DateTime.UtcNow;
            await _invoiceRepo.UpdateAsync(invoice);
        }

        await _unitOfWork.SaveChangesAsync();

        return new PaymentDto
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            Amount = payment.Amount,
            Currency = payment.Currency,
            PaymentMethod = payment.PaymentMethod.ToString(),
            Status = payment.Status.ToString(),
            TransactionReference = payment.TransactionReference,
            Notes = payment.Notes,
            ProcessedAt = payment.ProcessedAt,
            CreatedAt = payment.CreatedAt
        };
    }

    public async Task<BookkeepingReportDto> GetReportAsync(DateTime? from = null, DateTime? to = null)
    {
        var revenue = await _invoiceRepo.GetTotalRevenueAsync(from, to);
        var paidCount = await _invoiceRepo.GetCountByStatusAsync("Paid");
        var overdueCount = await _invoiceRepo.GetCountByStatusAsync("Overdue");
        var allInvoices = await _invoiceRepo.CountAsync();

        var outstanding = 0m;
        var issuedInvoices = await _invoiceRepo.FindAsync(i => i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.Overdue);
        outstanding = issuedInvoices.Sum(i => i.TotalAmount);

        return new BookkeepingReportDto
        {
            TotalRevenue = revenue,
            OutstandingAmount = outstanding,
            TotalInvoices = allInvoices,
            PaidInvoices = paidCount,
            OverdueInvoices = overdueCount
        };
    }

    public async Task<IEnumerable<InvoiceDto>> GetUserInvoicesAsync(Guid userId)
    {
        var invoices = await _invoiceRepo.GetByUserIdAsync(userId);
        return invoices.Select(MapToDto);
    }

    private static InvoiceDto MapToDto(Invoice i) => new()
    {
        Id = i.Id,
        InvoiceNumber = i.InvoiceNumber,
        UserId = i.UserId,
        UserName = i.User?.Username,
        ListingId = i.ListingId,
        SubscriptionId = i.SubscriptionId,
        Subtotal = i.Subtotal,
        TaxAmount = i.TaxAmount,
        TotalAmount = i.TotalAmount,
        Currency = i.Currency,
        Status = i.Status.ToString(),
        IssueDate = i.IssueDate,
        DueDate = i.DueDate,
        PaidDate = i.PaidDate,
        Notes = i.Notes,
        LineItems = i.LineItems.Select(li => new InvoiceLineItemDto
        {
            Id = li.Id,
            Description = li.Description,
            Quantity = li.Quantity,
            UnitPrice = li.UnitPrice,
            TotalPrice = li.TotalPrice
        }).ToList(),
        Payments = i.Payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            Amount = p.Amount,
            Currency = p.Currency,
            PaymentMethod = p.PaymentMethod.ToString(),
            Status = p.Status.ToString(),
            TransactionReference = p.TransactionReference,
            ProcessedAt = p.ProcessedAt,
            CreatedAt = p.CreatedAt
        }).ToList()
    };
}
