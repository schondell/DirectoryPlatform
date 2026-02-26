using DirectoryPlatform.Contracts.DTOs.Engagement;

namespace DirectoryPlatform.Contracts.Services;

public interface IListingEngagementService
{
    Task<ListingEngagementDto> GetEngagementAsync(Guid listingId, Guid? userId);
    Task<bool> ToggleLikeAsync(Guid listingId, Guid userId);
    Task<bool> ToggleFollowAsync(Guid listingId, Guid userId, bool notifyOnUpdate = true);
    Task TrackPageViewAsync(Guid listingId);
    Task TrackVisitorAsync(Guid listingId, Guid userId);
    Task<PageViewStatsDto> GetPageViewStatsAsync(Guid listingId, int days = 30);
    Task<VisitorStatsDto> GetVisitorStatsAsync(Guid listingId, int days = 30);
    Task<IEnumerable<LikedListingDto>> GetLikedListingsAsync(Guid userId);
    Task<IEnumerable<FollowedListingDto>> GetFollowedListingsAsync(Guid userId);
}
