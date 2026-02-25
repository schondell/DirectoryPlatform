using DirectoryPlatform.Core.Enums;

namespace DirectoryPlatform.Core.Entities;

public class ListingApprovalHistory : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public Guid? ReviewedByUserId { get; set; }
    public User? ReviewedByUser { get; set; }
    public ListingStatus OldStatus { get; set; }
    public ListingStatus NewStatus { get; set; }
    public string? Comment { get; set; }
}
