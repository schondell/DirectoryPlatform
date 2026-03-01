using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Boost;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class BoostServiceTests
{
    private readonly Mock<IBoostRepository> _boostRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly BoostService _boostService;

    public BoostServiceTests()
    {
        _boostRepoMock = new Mock<IBoostRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _boostService = new BoostService(_boostRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateBoostAsync_StandardBoost_CalculatesCorrectAmount()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateBoostDto
        {
            ListingId = Guid.NewGuid(),
            BoostType = "Standard",
            DurationDays = 7
        };

        _boostRepoMock.Setup(r => r.AddAsync(It.IsAny<ListingBoost>()))
            .ReturnsAsync((ListingBoost b) => b);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _boostService.CreateBoostAsync(dto, userId);

        result.Should().NotBeNull();
        result.BoostType.Should().Be("Standard");
        result.AmountPaid.Should().Be(2.90m * 7); // 20.30 CHF
        result.Multiplier.Should().Be(1.5);
        result.Currency.Should().Be("CHF");
    }

    [Fact]
    public async Task CreateBoostAsync_PremiumBoost_CalculatesCorrectAmount()
    {
        var dto = new CreateBoostDto
        {
            ListingId = Guid.NewGuid(),
            BoostType = "Premium",
            DurationDays = 14
        };

        _boostRepoMock.Setup(r => r.AddAsync(It.IsAny<ListingBoost>()))
            .ReturnsAsync((ListingBoost b) => b);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _boostService.CreateBoostAsync(dto, Guid.NewGuid());

        result.AmountPaid.Should().Be(5.90m * 14); // 82.60 CHF
        result.Multiplier.Should().Be(2.0);
    }

    [Fact]
    public async Task CreateBoostAsync_FeaturedBoost_CalculatesCorrectAmount()
    {
        var dto = new CreateBoostDto
        {
            ListingId = Guid.NewGuid(),
            BoostType = "Featured",
            DurationDays = 30
        };

        _boostRepoMock.Setup(r => r.AddAsync(It.IsAny<ListingBoost>()))
            .ReturnsAsync((ListingBoost b) => b);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _boostService.CreateBoostAsync(dto, Guid.NewGuid());

        result.AmountPaid.Should().Be(9.90m * 30); // 297.00 CHF
        result.Multiplier.Should().Be(3.0);
    }

    [Fact]
    public async Task CreateBoostAsync_InvalidBoostType_ThrowsArgumentException()
    {
        var dto = new CreateBoostDto
        {
            ListingId = Guid.NewGuid(),
            BoostType = "InvalidType",
            DurationDays = 7
        };

        var act = () => _boostService.CreateBoostAsync(dto, Guid.NewGuid());

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Invalid boost type: InvalidType");
    }

    [Fact]
    public async Task GetActiveBoostsAsync_ReturnsActiveBoosts()
    {
        var listingId = Guid.NewGuid();
        var boosts = new List<ListingBoost>
        {
            new()
            {
                Id = Guid.NewGuid(), ListingId = listingId, UserId = Guid.NewGuid(),
                BoostType = BoostType.Standard, StartsAt = DateTime.UtcNow.AddDays(-1),
                ExpiresAt = DateTime.UtcNow.AddDays(6), Multiplier = 1.5, AmountPaid = 20.30m
            }
        };
        _boostRepoMock.Setup(r => r.GetActiveByListingIdAsync(listingId)).ReturnsAsync(boosts);

        var result = await _boostService.GetActiveBoostsAsync(listingId);

        result.Should().HaveCount(1);
        result.First().BoostType.Should().Be("Standard");
    }

    [Fact]
    public async Task GetUserBoostsAsync_ReturnsUserBoosts()
    {
        var userId = Guid.NewGuid();
        var boosts = new List<ListingBoost>
        {
            new()
            {
                Id = Guid.NewGuid(), ListingId = Guid.NewGuid(), UserId = userId,
                BoostType = BoostType.Premium, StartsAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7), Multiplier = 2.0, AmountPaid = 41.30m
            }
        };
        _boostRepoMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(boosts);

        var result = await _boostService.GetUserBoostsAsync(userId);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPricingAsync_ReturnsAllThreeTiers()
    {
        var result = (await _boostService.GetPricingAsync()).ToList();

        result.Should().HaveCount(3);
        result.Should().Contain(p => p.BoostType == "Standard" && p.DailyRate == 2.90m);
        result.Should().Contain(p => p.BoostType == "Premium" && p.DailyRate == 5.90m);
        result.Should().Contain(p => p.BoostType == "Featured" && p.DailyRate == 9.90m);
    }

    [Fact]
    public async Task CreateBoostAsync_CaseInsensitiveBoostType_Works()
    {
        var dto = new CreateBoostDto
        {
            ListingId = Guid.NewGuid(),
            BoostType = "standard",
            DurationDays = 1
        };

        _boostRepoMock.Setup(r => r.AddAsync(It.IsAny<ListingBoost>()))
            .ReturnsAsync((ListingBoost b) => b);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _boostService.CreateBoostAsync(dto, Guid.NewGuid());

        result.BoostType.Should().Be("Standard");
        result.AmountPaid.Should().Be(2.90m);
    }
}
