using DirectoryPlatform.Contracts.DTOs.Boost;

namespace DirectoryPlatform.Contracts.Services;

public interface IBoostService
{
    Task<BoostDto> CreateBoostAsync(CreateBoostDto dto, Guid userId);
    Task<IEnumerable<BoostDto>> GetActiveBoostsAsync(Guid listingId);
    Task<IEnumerable<BoostDto>> GetUserBoostsAsync(Guid userId);
    Task<IEnumerable<BoostPricingDto>> GetPricingAsync();
}
