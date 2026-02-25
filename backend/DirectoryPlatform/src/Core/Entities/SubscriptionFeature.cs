namespace DirectoryPlatform.Core.Entities;

public class SubscriptionFeature : BaseEntity
{
    public Guid SubscriptionTierId { get; set; }
    public SubscriptionTier SubscriptionTier { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public bool IsEnabled { get; set; } = true;
}
