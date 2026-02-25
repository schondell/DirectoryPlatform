namespace DirectoryPlatform.Contracts.DTOs.Subscription;

public class SubscriptionDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SubscriptionTierId { get; set; }
    public string? TierName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool AutoRenew { get; set; }
    public DateTime CreatedAt { get; set; }
}
