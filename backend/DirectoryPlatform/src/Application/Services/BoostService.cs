using DirectoryPlatform.Contracts.DTOs.Boost;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class BoostService : IBoostService
{
    private readonly IBoostRepository _boostRepo;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly Dictionary<BoostType, (decimal DailyRate, double Multiplier, string Description)> BoostPricing = new()
    {
        { BoostType.Standard, (2.90m, 1.5, "Standard visibility boost") },
        { BoostType.Premium, (5.90m, 2.0, "Premium placement in search results") },
        { BoostType.Featured, (9.90m, 3.0, "Featured on homepage and top of category") }
    };

    public BoostService(IBoostRepository boostRepo, IUnitOfWork unitOfWork)
    {
        _boostRepo = boostRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<BoostDto> CreateBoostAsync(CreateBoostDto dto, Guid userId)
    {
        if (!Enum.TryParse<BoostType>(dto.BoostType, true, out var boostType))
            throw new ArgumentException($"Invalid boost type: {dto.BoostType}");

        var pricing = BoostPricing[boostType];
        var boost = new ListingBoost
        {
            Id = Guid.NewGuid(),
            ListingId = dto.ListingId,
            UserId = userId,
            BoostType = boostType,
            StartsAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(dto.DurationDays),
            Multiplier = pricing.Multiplier,
            AmountPaid = pricing.DailyRate * dto.DurationDays,
            Currency = "CHF"
        };

        await _boostRepo.AddAsync(boost);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(boost);
    }

    public async Task<IEnumerable<BoostDto>> GetActiveBoostsAsync(Guid listingId)
    {
        var boosts = await _boostRepo.GetActiveByListingIdAsync(listingId);
        return boosts.Select(MapToDto);
    }

    public async Task<IEnumerable<BoostDto>> GetUserBoostsAsync(Guid userId)
    {
        var boosts = await _boostRepo.GetByUserIdAsync(userId);
        return boosts.Select(MapToDto);
    }

    public Task<IEnumerable<BoostPricingDto>> GetPricingAsync()
    {
        var pricing = BoostPricing.Select(kv => new BoostPricingDto
        {
            BoostType = kv.Key.ToString(),
            DailyRate = kv.Value.DailyRate,
            Multiplier = kv.Value.Multiplier,
            Description = kv.Value.Description
        });
        return Task.FromResult(pricing);
    }

    private static BoostDto MapToDto(ListingBoost b) => new()
    {
        Id = b.Id,
        ListingId = b.ListingId,
        BoostType = b.BoostType.ToString(),
        StartsAt = b.StartsAt,
        ExpiresAt = b.ExpiresAt,
        Multiplier = b.Multiplier,
        AmountPaid = b.AmountPaid,
        Currency = b.Currency,
        IsActive = b.IsActive
    };
}
