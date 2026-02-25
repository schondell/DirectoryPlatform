namespace DirectoryPlatform.Core.Entities;

public class ListingAttribute : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public Guid AttributeDefinitionId { get; set; }
    public AttributeDefinition AttributeDefinition { get; set; } = null!;
    public string Value { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
