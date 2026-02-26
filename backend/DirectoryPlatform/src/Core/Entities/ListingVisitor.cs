namespace DirectoryPlatform.Core.Entities;

public class ListingVisitor : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
}
