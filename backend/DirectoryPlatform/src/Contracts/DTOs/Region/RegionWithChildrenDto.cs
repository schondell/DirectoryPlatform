namespace DirectoryPlatform.Contracts.DTOs.Region;

public class RegionWithChildrenDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? CountryCode { get; set; }
    public int DisplayOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<RegionWithChildrenDto> Children { get; set; } = new();
}
