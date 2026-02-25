using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class ListingMediaRepository : Repository<ListingMedia>, IListingMediaRepository
{
    public ListingMediaRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ListingMedia>> GetByListingIdAsync(Guid listingId)
        => await _dbSet.Where(m => m.ListingId == listingId).OrderBy(m => m.DisplayOrder).ToListAsync();
}
