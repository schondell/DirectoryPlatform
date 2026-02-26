namespace DirectoryPlatform.Core.Entities;

public class ListingPageView
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public DateTime ViewDate { get; set; }
    public int ViewCount { get; set; }
}
