namespace DirectoryPlatform.Core.Entities;

public class Region : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public Region? Parent { get; set; }
    public string? CountryCode { get; set; }
    public int DisplayOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }

    public ICollection<Region> Children { get; set; } = new List<Region>();
    public ICollection<Listing> Listings { get; set; } = new List<Listing>();
}
