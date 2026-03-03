using DirectoryPlatform.Contracts.DTOs.AiListing;

namespace DirectoryPlatform.Contracts.Services;

public interface IAiListingService
{
    Task<AiGeneratedListingDto> GenerateListingAsync(AiGenerateListingRequestDto request);
    Task<AiGeneratedListingDto> ImproveListingAsync(AiImproveListingRequestDto request);
}
