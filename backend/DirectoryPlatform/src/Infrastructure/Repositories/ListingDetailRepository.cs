using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class ListingDetailRepository : Repository<ListingDetail>, IListingDetailRepository
{
    public ListingDetailRepository(ApplicationDbContext context) : base(context) { }

    public async Task<ListingDetail?> GetByListingIdAsync(Guid listingId)
        => await _dbSet.FirstOrDefaultAsync(d => d.ListingId == listingId);
}
