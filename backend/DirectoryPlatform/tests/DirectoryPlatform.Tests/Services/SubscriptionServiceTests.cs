using AutoMapper;
using DirectoryPlatform.Application.Mapping;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Subscription;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class SubscriptionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISubscriptionRepository> _subscriptionRepoMock;
    private readonly IMapper _mapper;
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _subscriptionRepoMock = new Mock<ISubscriptionRepository>();
        _unitOfWorkMock.Setup(u => u.Subscriptions).Returns(_subscriptionRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _subscriptionService = new SubscriptionService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_HasActive_ReturnsSubscription()
    {
        var userId = Guid.NewGuid();
        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SubscriptionTierId = Guid.NewGuid(),
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            SubscriptionTier = new SubscriptionTier { Name = "Premium" }
        };
        _subscriptionRepoMock.Setup(r => r.GetActiveByUserIdAsync(userId)).ReturnsAsync(sub);

        var result = await _subscriptionService.GetActiveByUserIdAsync(userId);

        result.Should().NotBeNull();
        result!.IsActive.Should().BeTrue();
        result.TierName.Should().Be("Premium");
    }

    [Fact]
    public async Task GetActiveByUserIdAsync_NoActive_ReturnsNull()
    {
        _subscriptionRepoMock.Setup(r => r.GetActiveByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Subscription?)null);

        var result = await _subscriptionService.GetActiveByUserIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetTiersAsync_ReturnsTiers()
    {
        var tiers = new List<SubscriptionTier>
        {
            new() { Id = Guid.NewGuid(), Name = "Free", MonthlyPrice = 0, IsActive = true, Features = new List<SubscriptionFeature>() },
            new() { Id = Guid.NewGuid(), Name = "Standard", MonthlyPrice = 9.90m, IsActive = true, Features = new List<SubscriptionFeature>() }
        };
        _subscriptionRepoMock.Setup(r => r.GetActiveTiersAsync()).ReturnsAsync(tiers);

        var result = await _subscriptionService.GetTiersAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_ValidData_ReturnsSubscription()
    {
        var userId = Guid.NewGuid();
        var tierId = Guid.NewGuid();
        var dto = new CreateSubscriptionDto { SubscriptionTierId = tierId, AutoRenew = true };

        _subscriptionRepoMock.Setup(r => r.GetByIdAsync(tierId))
            .ReturnsAsync(new Subscription { Id = tierId });
        _subscriptionRepoMock.Setup(r => r.AddAsync(It.IsAny<Subscription>()))
            .ReturnsAsync((Subscription s) => s);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _subscriptionService.CreateAsync(dto, userId);

        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.SubscriptionTierId.Should().Be(tierId);
        result.IsActive.Should().BeTrue();
        result.AutoRenew.Should().BeTrue();
    }

    [Fact]
    public async Task CancelAsync_ByOwner_DeactivatesSubscription()
    {
        var userId = Guid.NewGuid();
        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsActive = true,
            AutoRenew = true
        };
        _subscriptionRepoMock.Setup(r => r.GetByIdAsync(sub.Id)).ReturnsAsync(sub);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _subscriptionService.CancelAsync(sub.Id, userId);

        sub.IsActive.Should().BeFalse();
        sub.AutoRenew.Should().BeFalse();
    }

    [Fact]
    public async Task CancelAsync_NotOwner_ThrowsUnauthorized()
    {
        var sub = new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            IsActive = true
        };
        _subscriptionRepoMock.Setup(r => r.GetByIdAsync(sub.Id)).ReturnsAsync(sub);

        var act = () => _subscriptionService.CancelAsync(sub.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task CancelAsync_NonExisting_ThrowsKeyNotFound()
    {
        _subscriptionRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Subscription?)null);

        var act = () => _subscriptionService.CancelAsync(Guid.NewGuid(), Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
