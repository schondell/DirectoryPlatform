namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class ListingAttributeValueDto
{
    public Guid AttributeDefinitionId { get; set; }
    public string Value { get; set; } = string.Empty;
}
