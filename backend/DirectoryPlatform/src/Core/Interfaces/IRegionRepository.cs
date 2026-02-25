using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IRegionRepository : IRepository<Region>
{
    Task<IEnumerable<Region>> GetRootRegionsAsync();
    Task<Region?> GetBySlugAsync(string slug);
    Task<Region?> GetWithChildrenAsync(Guid id);
    Task<IEnumerable<Region>> GetAllWithChildrenAsync();
}
