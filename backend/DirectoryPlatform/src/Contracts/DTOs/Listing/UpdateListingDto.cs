namespace DirectoryPlatform.Contracts.DTOs.Listing;

public class UpdateListingDto
{
    public string Title { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public Guid CategoryId { get; set; }
    public Guid? RegionId { get; set; }
    public string? Town { get; set; }
    public ListingDetailDto? Detail { get; set; }
    public List<ListingAttributeValueDto> Attributes { get; set; } = new();
}
