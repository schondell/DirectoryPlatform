using DirectoryPlatform.Contracts.DTOs.Region;

namespace DirectoryPlatform.Contracts.Services;

public interface IRegionService
{
    Task<IEnumerable<RegionDto>> GetAllAsync();
    Task<IEnumerable<RegionWithChildrenDto>> GetTreeAsync();
    Task<RegionWithChildrenDto?> GetByIdAsync(Guid id);
    Task<RegionDto?> GetBySlugAsync(string slug);
    Task<RegionDto> CreateAsync(CreateRegionDto dto);
    Task<RegionDto> UpdateAsync(Guid id, CreateRegionDto dto);
    Task DeleteAsync(Guid id);
}
