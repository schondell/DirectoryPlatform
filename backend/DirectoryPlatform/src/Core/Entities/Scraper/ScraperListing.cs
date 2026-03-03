namespace DirectoryPlatform.Core.Entities.Scraper;

public class ScraperListing
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? CategoryExternalId { get; set; }
    public string? PhoneRaw { get; set; }
    public string? PhoneNormalized { get; set; }
    public string? ImageHash { get; set; }
    public bool IsPaid { get; set; }
    public string? PaidType { get; set; }
    public string? ParsedData { get; set; }
    public bool ImagesDownloaded { get; set; }
    public string Status { get; set; } = "active";
    public DateTime FirstSeenAt { get; set; }
    public DateTime LastSeenAt { get; set; }
    public DateTime? ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
