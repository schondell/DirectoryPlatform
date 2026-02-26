namespace DirectoryPlatform.Core.Entities.Bookkeeping;

public class PaymentReconciliation : BaseEntity
{
    public Guid PaymentId { get; set; }
    public Payment Payment { get; set; } = null!;
    public Guid RecordedByUserId { get; set; }
    public User RecordedByUser { get; set; } = null!;
    public string Action { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
