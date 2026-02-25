using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IListingDetailRepository : IRepository<ListingDetail>
{
    Task<ListingDetail?> GetByListingIdAsync(Guid listingId);
}
