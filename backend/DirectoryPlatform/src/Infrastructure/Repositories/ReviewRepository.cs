using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Review>> GetByListingIdAsync(Guid listingId)
        => await _dbSet.Include(r => r.User).Where(r => r.ListingId == listingId).OrderByDescending(r => r.CreatedAt).ToListAsync();

    public async Task<double> GetAverageRatingAsync(Guid listingId)
    {
        var reviews = await _dbSet.Where(r => r.ListingId == listingId && r.Status == Core.Enums.ReviewStatus.Approved).ToListAsync();
        return reviews.Count != 0 ? reviews.Average(r => r.Rating) : 0;
    }
}
