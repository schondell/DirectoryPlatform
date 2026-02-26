namespace DirectoryPlatform.Contracts.DTOs.Engagement;

public class ListingEngagementDto
{
    public int LikeCount { get; set; }
    public int FollowerCount { get; set; }
    public bool HasUserLiked { get; set; }
    public bool IsUserFollowing { get; set; }
}

public class LikedListingDto
{
    public Guid ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime LikedAt { get; set; }
}

public class FollowedListingDto
{
    public Guid ListingId { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool NotifyOnUpdate { get; set; }
    public DateTime FollowedAt { get; set; }
}

public class PageViewStatsDto
{
    public int TotalViews { get; set; }
    public List<DailyViewDto> DailyViews { get; set; } = new();
}

public class DailyViewDto
{
    public DateTime Date { get; set; }
    public int ViewCount { get; set; }
}

public class VisitorDto
{
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime VisitedAt { get; set; }
}

public class VisitorStatsDto
{
    public int TotalVisitors { get; set; }
    public int UniqueVisitors { get; set; }
    public List<VisitorDto> RecentVisitors { get; set; } = new();
}
