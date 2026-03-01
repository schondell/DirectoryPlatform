using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class UserKpiServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IListingEngagementRepository> _engagementRepoMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<IListingRepository> _listingRepoMock;
    private readonly Mock<IMessageRepository> _messageRepoMock;
    private readonly Mock<IReviewRepository> _reviewRepoMock;
    private readonly UserKpiService _service;

    public UserKpiServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _engagementRepoMock = new Mock<IListingEngagementRepository>();
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _listingRepoMock = new Mock<IListingRepository>();
        _messageRepoMock = new Mock<IMessageRepository>();
        _reviewRepoMock = new Mock<IReviewRepository>();

        _unitOfWorkMock.Setup(u => u.Listings).Returns(_listingRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Messages).Returns(_messageRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Reviews).Returns(_reviewRepoMock.Object);

        _service = new UserKpiService(_unitOfWorkMock.Object, _engagementRepoMock.Object, _invoiceRepoMock.Object);
    }

    [Fact]
    public async Task GetSummaryAsync_NoListings_ReturnsZeroValues()
    {
        var userId = Guid.NewGuid();
        _listingRepoMock.Setup(r => r.GetByUserIdAsync(userId))
            .ReturnsAsync(new List<Listing>());
        _messageRepoMock.Setup(r => r.GetInboxAsync(userId))
            .ReturnsAsync(new List<Message>());
        _messageRepoMock.Setup(r => r.GetSentAsync(userId))
            .ReturnsAsync(new List<Message>());

        var result = await _service.GetSummaryAsync(userId);

        result.TotalListings.Should().Be(0);
        result.ActiveListings.Should().Be(0);
        result.TotalViews.Should().Be(0);
        result.TotalLikes.Should().Be(0);
        result.TotalFollowers.Should().Be(0);
        result.AverageRating.Should().Be(0);
    }

    [Fact]
    public async Task GetSummaryAsync_WithListings_ReturnsCorrectSummary()
    {
        var userId = Guid.NewGuid();
        var listingId1 = Guid.NewGuid();
        var listingId2 = Guid.NewGuid();
        var listings = new List<Listing>
        {
            new() { Id = listingId1, UserId = userId, Status = ListingStatus.Active, ViewCount = 100, Category = new Category { Name = "Cars" } },
            new() { Id = listingId2, UserId = userId, Status = ListingStatus.Draft, ViewCount = 50, Category = new Category { Name = "Electronics" } }
        };

        _listingRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(listings);
        _engagementRepoMock.Setup(r => r.GetLikeCountAsync(listingId1)).ReturnsAsync(10);
        _engagementRepoMock.Setup(r => r.GetLikeCountAsync(listingId2)).ReturnsAsync(5);
        _engagementRepoMock.Setup(r => r.GetFollowerCountAsync(listingId1)).ReturnsAsync(3);
        _engagementRepoMock.Setup(r => r.GetFollowerCountAsync(listingId2)).ReturnsAsync(2);
        _messageRepoMock.Setup(r => r.GetInboxAsync(userId))
            .ReturnsAsync(new List<Message>
            {
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() }
            });
        _messageRepoMock.Setup(r => r.GetSentAsync(userId))
            .ReturnsAsync(new List<Message>
            {
                new() { Id = Guid.NewGuid() }
            });
        _reviewRepoMock.Setup(r => r.GetAverageRatingAsync(listingId1)).ReturnsAsync(4.5);
        _reviewRepoMock.Setup(r => r.GetAverageRatingAsync(listingId2)).ReturnsAsync(0);

        var result = await _service.GetSummaryAsync(userId);

        result.TotalListings.Should().Be(2);
        result.ActiveListings.Should().Be(1);
        result.TotalViews.Should().Be(150);
        result.TotalLikes.Should().Be(15);
        result.TotalFollowers.Should().Be(5);
        result.TotalMessages.Should().Be(2);
        result.AverageRating.Should().Be(4.5); // only 1 listing has reviews
        result.ResponseRate.Should().Be(50.0); // 1 sent / 2 received * 100
    }

    [Fact]
    public async Task GetKpiDashboardAsync_ReturnsDashboardWithSummary()
    {
        var userId = Guid.NewGuid();
        var listingId = Guid.NewGuid();
        var listings = new List<Listing>
        {
            new()
            {
                Id = listingId, UserId = userId, Status = ListingStatus.Active, ViewCount = 50,
                Category = new Category { Name = "Vehicles" }
            }
        };

        _listingRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(listings);
        _engagementRepoMock.Setup(r => r.GetLikeCountAsync(listingId)).ReturnsAsync(5);
        _engagementRepoMock.Setup(r => r.GetFollowerCountAsync(listingId)).ReturnsAsync(3);
        _engagementRepoMock.Setup(r => r.GetPageViewsAsync(listingId, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(new List<ListingPageView>
            {
                new() { ListingId = listingId, ViewDate = DateTime.UtcNow.Date, ViewCount = 10 }
            });
        _messageRepoMock.Setup(r => r.GetInboxAsync(userId)).ReturnsAsync(new List<Message>());
        _messageRepoMock.Setup(r => r.GetSentAsync(userId)).ReturnsAsync(new List<Message>());
        _reviewRepoMock.Setup(r => r.GetAverageRatingAsync(listingId)).ReturnsAsync(4.0);

        var result = await _service.GetKpiDashboardAsync(userId, 30);

        result.Should().NotBeNull();
        result.Summary.TotalListings.Should().Be(1);
        result.ViewsOverTime.Should().HaveCount(1);
        result.CategoryPerformance.Should().HaveCount(1);
        result.CategoryPerformance[0].CategoryName.Should().Be("Vehicles");
    }

    [Fact]
    public async Task GetSummaryAsync_NoReceivedMessages_ResponseRateIsZero()
    {
        var userId = Guid.NewGuid();
        _listingRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(new List<Listing>());
        _messageRepoMock.Setup(r => r.GetInboxAsync(userId)).ReturnsAsync(new List<Message>());
        _messageRepoMock.Setup(r => r.GetSentAsync(userId)).ReturnsAsync(new List<Message> { new() { Id = Guid.NewGuid() } });

        var result = await _service.GetSummaryAsync(userId);

        result.ResponseRate.Should().Be(0);
    }
}
