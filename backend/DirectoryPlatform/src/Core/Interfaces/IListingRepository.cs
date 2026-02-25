using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IListingRepository : IRepository<Listing>
{
    Task<Listing?> GetWithDetailsAsync(Guid id);
    Task<(IEnumerable<Listing> Items, int TotalCount)> GetFilteredAsync(ListingFilterParams filter);
    Task<IEnumerable<Listing>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Listing>> GetFeaturedAsync(int count);
    Task<IEnumerable<Listing>> GetRecentAsync(int count);
    Task IncrementViewCountAsync(Guid id);
}
