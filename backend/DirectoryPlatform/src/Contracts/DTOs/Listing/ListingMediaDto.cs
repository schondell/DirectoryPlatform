namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class ListingMediaDto
{
    public Guid Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public string MediaType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
    public string? AltText { get; set; }
}
