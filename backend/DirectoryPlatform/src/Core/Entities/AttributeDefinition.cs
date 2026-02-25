using DirectoryPlatform.Core.Enums;

namespace DirectoryPlatform.Core.Entities;

public class AttributeDefinition : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public AttributeType Type { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string? Options { get; set; } // JSON array
    public bool IsFilterable { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public double? MinValue { get; set; }
    public double? MaxValue { get; set; }

    public ICollection<ListingAttribute> ListingAttributes { get; set; } = new List<ListingAttribute>();
}
