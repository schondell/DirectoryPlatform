using DirectoryPlatform.Contracts.DTOs.Subscription;

namespace DirectoryPlatform.Contracts.Services;

public interface ISubscriptionService
{
    Task<SubscriptionDto?> GetActiveByUserIdAsync(Guid userId);
    Task<IEnumerable<SubscriptionTierDto>> GetTiersAsync();
    Task<SubscriptionDto> CreateAsync(CreateSubscriptionDto dto, Guid userId);
    Task CancelAsync(Guid id, Guid userId);
}
