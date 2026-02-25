namespace DirectoryPlatform.Contracts.DTOs.Subscription;

public class CreateSubscriptionDto
{
    public Guid SubscriptionTierId { get; set; }
    public bool AutoRenew { get; set; }
    public string? CouponCode { get; set; }
}
