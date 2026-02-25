namespace DirectoryPlatform.Contracts.DTOs.Subscription;

public class SubscriptionTierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal? AnnualPrice { get; set; }
    public bool IsActive { get; set; }
    public List<SubscriptionFeatureDto> Features { get; set; } = new();
}
