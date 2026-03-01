using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class ListingEngagementServiceTests
{
    private readonly Mock<IListingEngagementRepository> _engagementRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ListingEngagementService _service;

    public ListingEngagementServiceTests()
    {
        _engagementRepoMock = new Mock<IListingEngagementRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _service = new ListingEngagementService(_engagementRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetEngagementAsync_WithUserId_ReturnsFullEngagement()
    {
        var listingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _engagementRepoMock.Setup(r => r.GetLikeCountAsync(listingId)).ReturnsAsync(10);
        _engagementRepoMock.Setup(r => r.GetFollowerCountAsync(listingId)).ReturnsAsync(5);
        _engagementRepoMock.Setup(r => r.GetLikeAsync(listingId, userId))
            .ReturnsAsync(new ListingLike { Id = Guid.NewGuid(), ListingId = listingId, UserId = userId });
        _engagementRepoMock.Setup(r => r.GetFollowerAsync(listingId, userId))
            .ReturnsAsync((ListingFollower?)null);

        var result = await _service.GetEngagementAsync(listingId, userId);

        result.LikeCount.Should().Be(10);
        result.FollowerCount.Should().Be(5);
        result.HasUserLiked.Should().BeTrue();
        result.IsUserFollowing.Should().BeFalse();
    }

    [Fact]
    public async Task GetEngagementAsync_WithoutUserId_ReturnsCountsOnly()
    {
        var listingId = Guid.NewGuid();

        _engagementRepoMock.Setup(r => r.GetLikeCountAsync(listingId)).ReturnsAsync(3);
        _engagementRepoMock.Setup(r => r.GetFollowerCountAsync(listingId)).ReturnsAsync(2);

        var result = await _service.GetEngagementAsync(listingId, null);

        result.LikeCount.Should().Be(3);
        result.FollowerCount.Should().Be(2);
        result.HasUserLiked.Should().BeFalse();
        result.IsUserFollowing.Should().BeFalse();
    }

    [Fact]
    public async Task ToggleLikeAsync_NotLiked_AddsLikeReturnsTrue()
    {
        var listingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _engagementRepoMock.Setup(r => r.GetLikeAsync(listingId, userId))
            .ReturnsAsync((ListingLike?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.ToggleLikeAsync(listingId, userId);

        result.Should().BeTrue();
        _engagementRepoMock.Verify(r => r.AddLikeAsync(It.Is<ListingLike>(l =>
            l.ListingId == listingId && l.UserId == userId)), Times.Once);
    }

    [Fact]
    public async Task ToggleLikeAsync_AlreadyLiked_RemovesLikeReturnsFalse()
    {
        var listingId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingLike = new ListingLike { Id = Guid.NewGuid(), ListingId = listingId, UserId = userId };

        _engagementRepoMock.Setup(r => r.GetLikeAsync(listingId, userId))
            .ReturnsAsync(existingLike);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.ToggleLikeAsync(listingId, userId);

        result.Should().BeFalse();
        _engagementRepoMock.Verify(r => r.RemoveLikeAsync(existingLike), Times.Once);
    }

    [Fact]
    public async Task ToggleFollowAsync_NotFollowing_AddsFollowerReturnsTrue()
    {
        var listingId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _engagementRepoMock.Setup(r => r.GetFollowerAsync(listingId, userId))
            .ReturnsAsync((ListingFollower?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.ToggleFollowAsync(listingId, userId, true);

        result.Should().BeTrue();
        _engagementRepoMock.Verify(r => r.AddFollowerAsync(It.Is<ListingFollower>(f =>
            f.ListingId == listingId && f.UserId == userId && f.NotifyOnUpdate == true)), Times.Once);
    }

    [Fact]
    public async Task ToggleFollowAsync_AlreadyFollowing_RemovesFollowerReturnsFalse()
    {
        var listingId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existing = new ListingFollower { Id = Guid.NewGuid(), ListingId = listingId, UserId = userId };

        _engagementRepoMock.Setup(r => r.GetFollowerAsync(listingId, userId))
            .ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _service.ToggleFollowAsync(listingId, userId);

        result.Should().BeFalse();
        _engagementRepoMock.Verify(r => r.RemoveFollowerAsync(existing), Times.Once);
    }

    [Fact]
    public async Task TrackPageViewAsync_CallsRepositoryAndSaves()
    {
        var listingId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.TrackPageViewAsync(listingId);

        _engagementRepoMock.Verify(r => r.AddOrIncrementPageViewAsync(listingId, It.IsAny<DateTime>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task TrackVisitorAsync_CallsRepositoryAndSaves()
    {
        var listingId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _service.TrackVisitorAsync(listingId, userId);

        _engagementRepoMock.Verify(r => r.AddVisitorAsync(It.Is<ListingVisitor>(v =>
            v.ListingId == listingId && v.UserId == userId)), Times.Once);
    }

    [Fact]
    public async Task GetPageViewStatsAsync_ReturnsStats()
    {
        var listingId = Guid.NewGuid();
        var views = new List<ListingPageView>
        {
            new() { ListingId = listingId, ViewDate = DateTime.UtcNow.Date.AddDays(-1), ViewCount = 10 },
            new() { ListingId = listingId, ViewDate = DateTime.UtcNow.Date, ViewCount = 5 }
        };

        _engagementRepoMock.Setup(r => r.GetPageViewsAsync(listingId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(views);

        var result = await _service.GetPageViewStatsAsync(listingId, 30);

        result.TotalViews.Should().Be(15);
        result.DailyViews.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetVisitorStatsAsync_ReturnsStats()
    {
        var listingId = Guid.NewGuid();
        var visitors = new List<ListingVisitor>
        {
            new() { Id = Guid.NewGuid(), ListingId = listingId, UserId = Guid.NewGuid(), VisitedAt = DateTime.UtcNow, User = new User { Username = "visitor1" } },
            new() { Id = Guid.NewGuid(), ListingId = listingId, UserId = Guid.NewGuid(), VisitedAt = DateTime.UtcNow, User = new User { Username = "visitor2" } }
        };

        _engagementRepoMock.Setup(r => r.GetVisitorsAsync(listingId, 50)).ReturnsAsync(visitors);
        _engagementRepoMock.Setup(r => r.GetUniqueVisitorCountAsync(listingId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(2);

        var result = await _service.GetVisitorStatsAsync(listingId, 30);

        result.TotalVisitors.Should().Be(2);
        result.UniqueVisitors.Should().Be(2);
        result.RecentVisitors.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetLikedListingsAsync_ReturnsLikedListings()
    {
        var userId = Guid.NewGuid();
        var likes = new List<ListingLike>
        {
            new() { Id = Guid.NewGuid(), ListingId = Guid.NewGuid(), UserId = userId, Listing = new Listing { Title = "Listing 1" }, CreatedAt = DateTime.UtcNow }
        };
        _engagementRepoMock.Setup(r => r.GetUserLikesAsync(userId)).ReturnsAsync(likes);

        var result = (await _service.GetLikedListingsAsync(userId)).ToList();

        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Listing 1");
    }

    [Fact]
    public async Task GetFollowedListingsAsync_ReturnsFollowedListings()
    {
        var userId = Guid.NewGuid();
        var follows = new List<ListingFollower>
        {
            new()
            {
                Id = Guid.NewGuid(), ListingId = Guid.NewGuid(), UserId = userId,
                Listing = new Listing { Title = "Followed" }, NotifyOnUpdate = true,
                CreatedAt = DateTime.UtcNow
            }
        };
        _engagementRepoMock.Setup(r => r.GetUserFollowsAsync(userId)).ReturnsAsync(follows);

        var result = (await _service.GetFollowedListingsAsync(userId)).ToList();

        result.Should().HaveCount(1);
        result[0].Title.Should().Be("Followed");
        result[0].NotifyOnUpdate.Should().BeTrue();
    }
}
