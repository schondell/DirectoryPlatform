namespace DirectoryPlatform.Core.Entities.Scraper;

public class ScraperCategory
{
    public int Id { get; set; }
    public string ExternalId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
    public string? ParentExternalId { get; set; }
    public int ListingCount { get; set; }
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; }
}
