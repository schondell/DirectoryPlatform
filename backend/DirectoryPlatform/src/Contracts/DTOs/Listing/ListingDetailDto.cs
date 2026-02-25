namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class ListingDetailDto
{
    public string? Address { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? AvailabilityHours { get; set; }
    public string? PriceInfo { get; set; }
    public string? PaymentMethods { get; set; }
}
