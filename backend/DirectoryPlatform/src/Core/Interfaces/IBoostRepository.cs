using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IBoostRepository : IRepository<ListingBoost>
{
    Task<IEnumerable<ListingBoost>> GetActiveByListingIdAsync(Guid listingId);
    Task<IEnumerable<ListingBoost>> GetByUserIdAsync(Guid userId);
}
