namespace DirectoryPlatform.Core.Entities;

public class PricingTier : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DurationDays { get; set; }
    public int MaxListings { get; set; }
    public int MaxPhotos { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsActive { get; set; } = true;
}
