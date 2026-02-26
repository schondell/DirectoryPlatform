using DirectoryPlatform.Core.Enums;

namespace DirectoryPlatform.Core.Entities;

public class ListingBoost : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public BoostType BoostType { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public double Multiplier { get; set; } = 1.0;
    public decimal AmountPaid { get; set; }
    public string Currency { get; set; } = "CHF";
    public bool IsActive => DateTime.UtcNow >= StartsAt && DateTime.UtcNow <= ExpiresAt;
}
