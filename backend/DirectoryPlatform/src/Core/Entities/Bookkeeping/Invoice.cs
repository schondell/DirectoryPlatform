using DirectoryPlatform.Core.Enums;

namespace DirectoryPlatform.Core.Entities.Bookkeeping;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? ListingId { get; set; }
    public Listing? Listing { get; set; }
    public Guid? SubscriptionId { get; set; }
    public Subscription? Subscription { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "CHF";
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    public ICollection<InvoiceLineItem> LineItems { get; set; } = new List<InvoiceLineItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
