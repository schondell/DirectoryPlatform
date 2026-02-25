using DirectoryPlatform.Contracts.DTOs.Common;
using DirectoryPlatform.Contracts.DTOs.Listing;

namespace DirectoryPlatform.Contracts.Services;

public interface IListingService
{
    Task<ListingDto?> GetByIdAsync(Guid id);
    Task<PagedResultDto<ListingDto>> GetFilteredListingsAsync(ListingFilterRequestDto filter);
    Task<IEnumerable<ListingDto>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<ListingDto>> GetFeaturedAsync(int count = 10);
    Task<IEnumerable<ListingDto>> GetRecentAsync(int count = 10);
    Task<ListingDto> CreateAsync(CreateListingDto dto, Guid userId);
    Task<ListingDto> UpdateAsync(Guid id, UpdateListingDto dto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<ListingDto> UpdateStatusAsync(Guid id, string status, Guid adminUserId, string? comment = null);
    Task IncrementViewCountAsync(Guid id);
}
