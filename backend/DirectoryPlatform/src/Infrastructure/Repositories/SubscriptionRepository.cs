using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    public SubscriptionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Subscription?> GetActiveByUserIdAsync(Guid userId)
        => await _dbSet.Include(s => s.SubscriptionTier).FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive && s.EndDate > DateTime.UtcNow);

    public async Task<IEnumerable<Subscription>> GetExpiringAsync(int daysUntilExpiry)
    {
        var expiryDate = DateTime.UtcNow.AddDays(daysUntilExpiry);
        return await _dbSet.Include(s => s.User).Include(s => s.SubscriptionTier).Where(s => s.IsActive && s.EndDate <= expiryDate && s.EndDate > DateTime.UtcNow).ToListAsync();
    }

    public async Task<IEnumerable<SubscriptionTier>> GetActiveTiersAsync()
        => await _context.SubscriptionTiers.Include(t => t.Features).Where(t => t.IsActive).OrderBy(t => t.DisplayOrder).ToListAsync();
}
