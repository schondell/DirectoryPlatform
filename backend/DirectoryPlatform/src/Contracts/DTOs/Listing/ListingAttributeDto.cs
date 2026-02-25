namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class ListingAttributeDto
{
    public Guid Id { get; set; }
    public Guid AttributeDefinitionId { get; set; }
    public string AttributeName { get; set; } = string.Empty;
    public string AttributeSlug { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public string? Unit { get; set; }
}
