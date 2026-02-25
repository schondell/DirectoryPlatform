namespace DirectoryPlatform.Contracts.DTOs.Review;

public class CreateReviewDto
{
    public Guid ListingId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
