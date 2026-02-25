using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IReviewRepository : IRepository<Review>
{
    Task<IEnumerable<Review>> GetByListingIdAsync(Guid listingId);
    Task<double> GetAverageRatingAsync(Guid listingId);
}
