namespace DirectoryPlatform.Core.Entities;

public class SubscriptionTier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal? AnnualPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }

    public ICollection<SubscriptionFeature> Features { get; set; } = new List<SubscriptionFeature>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
