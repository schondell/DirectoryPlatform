using DirectoryPlatform.Core.Enums;

namespace DirectoryPlatform.Core.Entities;

public class Listing : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public ListingStatus Status { get; set; } = ListingStatus.Draft;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }
    public string? Town { get; set; }
    public int Weight { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPremium { get; set; }
    public int ViewCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public ListingDetail? Detail { get; set; }
    public ICollection<ListingAttribute> Attributes { get; set; } = new List<ListingAttribute>();
    public ICollection<ListingMedia> Media { get; set; } = new List<ListingMedia>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<ListingLanguage> Languages { get; set; } = new List<ListingLanguage>();
    public ICollection<ListingApprovalHistory> ApprovalHistory { get; set; } = new List<ListingApprovalHistory>();
}
