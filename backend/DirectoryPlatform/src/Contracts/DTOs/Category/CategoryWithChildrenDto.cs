namespace DirectoryPlatform.Contracts.DTOs.Category;

public class CategoryWithChildrenDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public Guid? ParentId { get; set; }
    public string? IconUrl { get; set; }
    public int DisplayOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public List<CategoryWithChildrenDto> Children { get; set; } = new();
}
