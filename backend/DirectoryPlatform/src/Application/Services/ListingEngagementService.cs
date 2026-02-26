using DirectoryPlatform.Contracts.DTOs.Engagement;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class ListingEngagementService : IListingEngagementService
{
    private readonly IListingEngagementRepository _engagementRepo;
    private readonly IUnitOfWork _unitOfWork;

    public ListingEngagementService(IListingEngagementRepository engagementRepo, IUnitOfWork unitOfWork)
    {
        _engagementRepo = engagementRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ListingEngagementDto> GetEngagementAsync(Guid listingId, Guid? userId)
    {
        var dto = new ListingEngagementDto
        {
            LikeCount = await _engagementRepo.GetLikeCountAsync(listingId),
            FollowerCount = await _engagementRepo.GetFollowerCountAsync(listingId)
        };

        if (userId.HasValue)
        {
            dto.HasUserLiked = await _engagementRepo.GetLikeAsync(listingId, userId.Value) != null;
            dto.IsUserFollowing = await _engagementRepo.GetFollowerAsync(listingId, userId.Value) != null;
        }

        return dto;
    }

    public async Task<bool> ToggleLikeAsync(Guid listingId, Guid userId)
    {
        var existing = await _engagementRepo.GetLikeAsync(listingId, userId);
        if (existing != null)
        {
            await _engagementRepo.RemoveLikeAsync(existing);
            await _unitOfWork.SaveChangesAsync();
            return false;
        }

        await _engagementRepo.AddLikeAsync(new ListingLike { Id = Guid.NewGuid(), ListingId = listingId, UserId = userId });
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleFollowAsync(Guid listingId, Guid userId, bool notifyOnUpdate = true)
    {
        var existing = await _engagementRepo.GetFollowerAsync(listingId, userId);
        if (existing != null)
        {
            await _engagementRepo.RemoveFollowerAsync(existing);
            await _unitOfWork.SaveChangesAsync();
            return false;
        }

        await _engagementRepo.AddFollowerAsync(new ListingFollower { Id = Guid.NewGuid(), ListingId = listingId, UserId = userId, NotifyOnUpdate = notifyOnUpdate });
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task TrackPageViewAsync(Guid listingId)
    {
        await _engagementRepo.AddOrIncrementPageViewAsync(listingId, DateTime.UtcNow);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task TrackVisitorAsync(Guid listingId, Guid userId)
    {
        await _engagementRepo.AddVisitorAsync(new ListingVisitor { Id = Guid.NewGuid(), ListingId = listingId, UserId = userId, VisitedAt = DateTime.UtcNow });
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PageViewStatsDto> GetPageViewStatsAsync(Guid listingId, int days = 30)
    {
        var from = DateTime.UtcNow.Date.AddDays(-days);
        var to = DateTime.UtcNow.Date;
        var views = await _engagementRepo.GetPageViewsAsync(listingId, from, to);

        return new PageViewStatsDto
        {
            TotalViews = views.Sum(v => v.ViewCount),
            DailyViews = views.Select(v => new DailyViewDto { Date = v.ViewDate, ViewCount = v.ViewCount }).ToList()
        };
    }

    public async Task<VisitorStatsDto> GetVisitorStatsAsync(Guid listingId, int days = 30)
    {
        var from = DateTime.UtcNow.AddDays(-days);
        var to = DateTime.UtcNow;
        var visitors = await _engagementRepo.GetVisitorsAsync(listingId, 50);
        var uniqueCount = await _engagementRepo.GetUniqueVisitorCountAsync(listingId, from, to);

        return new VisitorStatsDto
        {
            TotalVisitors = visitors.Count(),
            UniqueVisitors = uniqueCount,
            RecentVisitors = visitors.Select(v => new VisitorDto
            {
                UserId = v.UserId,
                UserName = v.User?.Username,
                VisitedAt = v.VisitedAt
            }).ToList()
        };
    }

    public async Task<IEnumerable<LikedListingDto>> GetLikedListingsAsync(Guid userId)
    {
        var likes = await _engagementRepo.GetUserLikesAsync(userId);
        return likes.Select(l => new LikedListingDto
        {
            ListingId = l.ListingId,
            Title = l.Listing?.Title ?? string.Empty,
            LikedAt = l.CreatedAt
        });
    }

    public async Task<IEnumerable<FollowedListingDto>> GetFollowedListingsAsync(Guid userId)
    {
        var follows = await _engagementRepo.GetUserFollowsAsync(userId);
        return follows.Select(f => new FollowedListingDto
        {
            ListingId = f.ListingId,
            Title = f.Listing?.Title ?? string.Empty,
            NotifyOnUpdate = f.NotifyOnUpdate,
            FollowedAt = f.CreatedAt
        });
    }
}
