namespace DirectoryPlatform.Contracts.DTOs.Subscription;

public class SubscriptionFeatureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public bool IsEnabled { get; set; }
}
