using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface ISubscriptionRepository : IRepository<Subscription>
{
    Task<Subscription?> GetActiveByUserIdAsync(Guid userId);
    Task<IEnumerable<Subscription>> GetExpiringAsync(int daysUntilExpiry);
    Task<IEnumerable<SubscriptionTier>> GetActiveTiersAsync();
}
