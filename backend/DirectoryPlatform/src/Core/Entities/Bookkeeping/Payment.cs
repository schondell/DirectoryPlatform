using DirectoryPlatform.Core.Enums;

namespace DirectoryPlatform.Core.Entities.Bookkeeping;

public class Payment : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Invoice Invoice { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "CHF";
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? TransactionReference { get; set; }
    public string? Notes { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
