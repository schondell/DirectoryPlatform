namespace DirectoryPlatform.Contracts.DTOs.Bookkeeping;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public Guid? ListingId { get; set; }
    public Guid? SubscriptionId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "CHF";
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}

public class InvoiceSummaryDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "CHF";
    public string Status { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
}

public class InvoiceLineItemDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CreateInvoiceRequest
{
    public Guid UserId { get; set; }
    public Guid? ListingId { get; set; }
    public Guid? SubscriptionId { get; set; }
    public DateTime DueDate { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceLineItemRequest> LineItems { get; set; } = new();
}

public class CreateInvoiceLineItemRequest
{
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}

public class UpdateInvoiceStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class InvoiceSearchRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? UserId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "CHF";
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentRequest
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
}

public class BookkeepingReportDto
{
    public decimal TotalRevenue { get; set; }
    public decimal OutstandingAmount { get; set; }
    public int TotalInvoices { get; set; }
    public int PaidInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
}

public class MonthlyRevenueDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal Revenue { get; set; }
    public int InvoiceCount { get; set; }
}
