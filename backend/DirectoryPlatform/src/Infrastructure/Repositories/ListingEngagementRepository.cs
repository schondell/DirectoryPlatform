using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class ListingEngagementRepository : IListingEngagementRepository
{
    private readonly ApplicationDbContext _context;

    public ListingEngagementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ListingLike?> GetLikeAsync(Guid listingId, Guid userId)
        => await _context.Set<ListingLike>().FirstOrDefaultAsync(l => l.ListingId == listingId && l.UserId == userId);

    public async Task<int> GetLikeCountAsync(Guid listingId)
        => await _context.Set<ListingLike>().CountAsync(l => l.ListingId == listingId);

    public async Task AddLikeAsync(ListingLike like)
        => await _context.Set<ListingLike>().AddAsync(like);

    public async Task RemoveLikeAsync(ListingLike like)
    {
        _context.Set<ListingLike>().Remove(like);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<ListingLike>> GetUserLikesAsync(Guid userId)
        => await _context.Set<ListingLike>().Include(l => l.Listing).Where(l => l.UserId == userId).OrderByDescending(l => l.CreatedAt).ToListAsync();

    public async Task<ListingFollower?> GetFollowerAsync(Guid listingId, Guid userId)
        => await _context.Set<ListingFollower>().FirstOrDefaultAsync(f => f.ListingId == listingId && f.UserId == userId);

    public async Task<int> GetFollowerCountAsync(Guid listingId)
        => await _context.Set<ListingFollower>().CountAsync(f => f.ListingId == listingId);

    public async Task AddFollowerAsync(ListingFollower follower)
        => await _context.Set<ListingFollower>().AddAsync(follower);

    public async Task RemoveFollowerAsync(ListingFollower follower)
    {
        _context.Set<ListingFollower>().Remove(follower);
        await Task.CompletedTask;
    }

    public async Task<IEnumerable<ListingFollower>> GetUserFollowsAsync(Guid userId)
        => await _context.Set<ListingFollower>().Include(f => f.Listing).Where(f => f.UserId == userId).OrderByDescending(f => f.CreatedAt).ToListAsync();

    public async Task AddOrIncrementPageViewAsync(Guid listingId, DateTime date)
    {
        var dateOnly = date.Date;
        var existing = await _context.Set<ListingPageView>().FirstOrDefaultAsync(p => p.ListingId == listingId && p.ViewDate == dateOnly);
        if (existing != null)
        {
            existing.ViewCount++;
        }
        else
        {
            await _context.Set<ListingPageView>().AddAsync(new ListingPageView { ListingId = listingId, ViewDate = dateOnly, ViewCount = 1 });
        }
    }

    public async Task<IEnumerable<ListingPageView>> GetPageViewsAsync(Guid listingId, DateTime from, DateTime to)
        => await _context.Set<ListingPageView>().Where(p => p.ListingId == listingId && p.ViewDate >= from && p.ViewDate <= to).OrderBy(p => p.ViewDate).ToListAsync();

    public async Task<int> GetTotalPageViewsAsync(Guid listingId)
        => await _context.Set<ListingPageView>().Where(p => p.ListingId == listingId).SumAsync(p => p.ViewCount);

    public async Task AddVisitorAsync(ListingVisitor visitor)
        => await _context.Set<ListingVisitor>().AddAsync(visitor);

    public async Task<IEnumerable<ListingVisitor>> GetVisitorsAsync(Guid listingId, int count = 50)
        => await _context.Set<ListingVisitor>().Include(v => v.User).Where(v => v.ListingId == listingId).OrderByDescending(v => v.VisitedAt).Take(count).ToListAsync();

    public async Task<int> GetUniqueVisitorCountAsync(Guid listingId, DateTime from, DateTime to)
        => await _context.Set<ListingVisitor>().Where(v => v.ListingId == listingId && v.VisitedAt >= from && v.VisitedAt <= to).Select(v => v.UserId).Distinct().CountAsync();
}
