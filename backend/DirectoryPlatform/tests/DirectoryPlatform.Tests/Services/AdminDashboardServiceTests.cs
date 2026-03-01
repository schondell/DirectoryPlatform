using System.Linq.Expressions;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class AdminDashboardServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IInvoiceRepository> _invoiceRepoMock;
    private readonly Mock<IVisitorMetricRepository> _visitorMetricRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IListingRepository> _listingRepoMock;
    private readonly AdminDashboardService _service;

    public AdminDashboardServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _visitorMetricRepoMock = new Mock<IVisitorMetricRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _listingRepoMock = new Mock<IListingRepository>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Listings).Returns(_listingRepoMock.Object);

        _service = new AdminDashboardService(_unitOfWorkMock.Object, _invoiceRepoMock.Object, _visitorMetricRepoMock.Object);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsOverviewMetrics()
    {
        _userRepoMock.Setup(r => r.CountAsync(null)).ReturnsAsync(100);
        _listingRepoMock.Setup(r => r.CountAsync(null)).ReturnsAsync(500);
        _listingRepoMock.Setup(r => r.CountAsync(It.Is<Expression<Func<Listing, bool>>>(e => true)))
            .ReturnsAsync(200);
        _invoiceRepoMock.Setup(r => r.GetTotalRevenueAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(5000m);
        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>());
        _listingRepoMock.Setup(r => r.GetRecentAsync(10))
            .ReturnsAsync(new List<Listing>());

        var result = await _service.GetDashboardAsync();

        result.Should().NotBeNull();
        result.Overview.Should().NotBeNull();
        result.RecentActivity.Should().NotBeNull();
        result.SystemHealth.Should().NotBeNull();
        result.SystemHealth.DotNetVersion.Should().NotBeNullOrEmpty();
        result.SystemHealth.OsPlatform.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsRecentActivity()
    {
        _userRepoMock.Setup(r => r.CountAsync(null)).ReturnsAsync(0);
        _listingRepoMock.Setup(r => r.CountAsync(null)).ReturnsAsync(0);
        _listingRepoMock.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Listing, bool>>>()))
            .ReturnsAsync(0);
        _invoiceRepoMock.Setup(r => r.GetTotalRevenueAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(0m);
        _userRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(new List<User>
            {
                new() { Id = Guid.NewGuid(), Username = "user1", Email = "user1@test.com", CreatedAt = DateTime.UtcNow }
            });
        _listingRepoMock.Setup(r => r.GetRecentAsync(10))
            .ReturnsAsync(new List<Listing>
            {
                new()
                {
                    Id = Guid.NewGuid(), Title = "Recent", Status = ListingStatus.Active, CreatedAt = DateTime.UtcNow,
                    Category = new Category { Name = "Cars" }
                }
            });

        var result = await _service.GetDashboardAsync();

        result.RecentActivity.RecentUsers.Should().HaveCount(1);
        result.RecentActivity.RecentListings.Should().HaveCount(1);
        result.RecentActivity.RecentListings[0].Title.Should().Be("Recent");
    }

    [Fact]
    public async Task GetVisitorMetricsAsync_ReturnsMetrics()
    {
        var today = DateTime.UtcNow.Date;
        var metrics = new List<VisitorMetric>
        {
            new() { Id = Guid.NewGuid(), Date = today, UniqueVisitors = 100, TotalPageViews = 500, NewUsers = 10, NewListings = 5 },
            new() { Id = Guid.NewGuid(), Date = today.AddDays(-1), UniqueVisitors = 80, TotalPageViews = 400, NewUsers = 8, NewListings = 3 }
        };

        _visitorMetricRepoMock.Setup(r => r.GetMetricsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(metrics);

        var result = await _service.GetVisitorMetricsAsync(30);

        result.TotalVisitorsToday.Should().Be(100);
        result.TotalPageViewsToday.Should().Be(500);
        result.DailyMetrics.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetVisitorMetricsAsync_NoMetricsToday_ReturnsZeroForToday()
    {
        var yesterday = DateTime.UtcNow.Date.AddDays(-1);
        var metrics = new List<VisitorMetric>
        {
            new() { Id = Guid.NewGuid(), Date = yesterday, UniqueVisitors = 50, TotalPageViews = 200, NewUsers = 5, NewListings = 2 }
        };

        _visitorMetricRepoMock.Setup(r => r.GetMetricsAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(metrics);

        var result = await _service.GetVisitorMetricsAsync(30);

        result.TotalVisitorsToday.Should().Be(0);
        result.TotalPageViewsToday.Should().Be(0);
        result.DailyMetrics.Should().HaveCount(1);
    }
}
