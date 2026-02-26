using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IListingEngagementRepository
{
    // Likes
    Task<ListingLike?> GetLikeAsync(Guid listingId, Guid userId);
    Task<int> GetLikeCountAsync(Guid listingId);
    Task AddLikeAsync(ListingLike like);
    Task RemoveLikeAsync(ListingLike like);
    Task<IEnumerable<ListingLike>> GetUserLikesAsync(Guid userId);

    // Followers
    Task<ListingFollower?> GetFollowerAsync(Guid listingId, Guid userId);
    Task<int> GetFollowerCountAsync(Guid listingId);
    Task AddFollowerAsync(ListingFollower follower);
    Task RemoveFollowerAsync(ListingFollower follower);
    Task<IEnumerable<ListingFollower>> GetUserFollowsAsync(Guid userId);

    // Page Views
    Task AddOrIncrementPageViewAsync(Guid listingId, DateTime date);
    Task<IEnumerable<ListingPageView>> GetPageViewsAsync(Guid listingId, DateTime from, DateTime to);
    Task<int> GetTotalPageViewsAsync(Guid listingId);

    // Visitors
    Task AddVisitorAsync(ListingVisitor visitor);
    Task<IEnumerable<ListingVisitor>> GetVisitorsAsync(Guid listingId, int count = 50);
    Task<int> GetUniqueVisitorCountAsync(Guid listingId, DateTime from, DateTime to);
}
