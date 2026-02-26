namespace DirectoryPlatform.Core.Entities;

public class ListingFollower : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public bool NotifyOnUpdate { get; set; } = true;
}
