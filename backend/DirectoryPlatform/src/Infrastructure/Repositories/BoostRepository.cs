using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class BoostRepository : Repository<ListingBoost>, IBoostRepository
{
    public BoostRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ListingBoost>> GetActiveByListingIdAsync(Guid listingId)
    {
        var now = DateTime.UtcNow;
        return await _dbSet.Where(b => b.ListingId == listingId && b.StartsAt <= now && b.ExpiresAt >= now).ToListAsync();
    }

    public async Task<IEnumerable<ListingBoost>> GetByUserIdAsync(Guid userId)
        => await _dbSet.Include(b => b.Listing).Where(b => b.UserId == userId).OrderByDescending(b => b.CreatedAt).ToListAsync();
}
