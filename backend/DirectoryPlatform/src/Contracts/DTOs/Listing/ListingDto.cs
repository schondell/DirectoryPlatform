using DirectoryPlatform.Contracts.DTOs.Category;
using DirectoryPlatform.Contracts.DTOs.Region;

namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class ListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public CategoryDto? Category { get; set; }
    public Guid? RegionId { get; set; }
    public RegionDto? Region { get; set; }
    public string? Town { get; set; }
    public int Weight { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPremium { get; set; }
    public int ViewCount { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ListingDetailDto? Detail { get; set; }
    public List<ListingAttributeDto> Attributes { get; set; } = new();
    public List<ListingMediaDto> Media { get; set; } = new();
}
