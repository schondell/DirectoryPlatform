namespace DirectoryPlatform.Core.Entities;

public class Subscription : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid SubscriptionTierId { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool AutoRenew { get; set; }
    public string? PaymentReference { get; set; }
    public Guid? CouponCodeId { get; set; }
    public CouponCode? CouponCode { get; set; }
}
