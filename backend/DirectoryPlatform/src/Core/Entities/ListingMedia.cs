using DirectoryPlatform.Core.Enums;

namespace DirectoryPlatform.Core.Entities;

public class ListingMedia : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public string Url { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
    public string? AltText { get; set; }
}
