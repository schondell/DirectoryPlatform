namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class UpdateAttributeDefinitionDto
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string[]? Options { get; set; }
    public bool IsFilterable { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }
}
