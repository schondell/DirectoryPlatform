namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class ListingFilterRequestDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? RegionId { get; set; }
    public string? SortBy { get; set; }
    public bool Ascending { get; set; } = true;
    public Dictionary<string, string> Attributes { get; set; } = new();
}
