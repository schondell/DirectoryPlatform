namespace DirectoryPlatform.Core.Entities;

public class ListingDetail : BaseEntity
{
    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? AvailabilityHours { get; set; } // JSON
    public string? PriceInfo { get; set; } // JSON
    public string? PaymentMethods { get; set; } // JSON
}
