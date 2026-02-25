using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IListingAttributeRepository : IRepository<ListingAttribute>
{
    Task<IEnumerable<ListingAttribute>> GetByListingIdAsync(Guid listingId);
}
