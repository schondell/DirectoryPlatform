using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class ListingAttributeRepository : Repository<ListingAttribute>, IListingAttributeRepository
{
    public ListingAttributeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ListingAttribute>> GetByListingIdAsync(Guid listingId)
        => await _dbSet.Include(a => a.AttributeDefinition).Where(a => a.ListingId == listingId).OrderBy(a => a.DisplayOrder).ToListAsync();
}
