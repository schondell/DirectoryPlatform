using DirectoryPlatform.Contracts.DTOs.Review;

namespace DirectoryPlatform.Contracts.Services;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetByListingIdAsync(Guid listingId);
    Task<ReviewDto> CreateAsync(CreateReviewDto dto, Guid userId);
    Task<ReviewDto> UpdateStatusAsync(Guid id, string status);
    Task DeleteAsync(Guid id);
    Task<double> GetAverageRatingAsync(Guid listingId);
}
