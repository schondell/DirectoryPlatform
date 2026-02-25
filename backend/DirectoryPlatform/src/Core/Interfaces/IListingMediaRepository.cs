using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IListingMediaRepository : IRepository<ListingMedia>
{
    Task<IEnumerable<ListingMedia>> GetByListingIdAsync(Guid listingId);
}
