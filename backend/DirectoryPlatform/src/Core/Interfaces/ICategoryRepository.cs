using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetRootCategoriesAsync();
    Task<Category?> GetBySlugAsync(string slug);
    Task<Category?> GetWithChildrenAsync(Guid id);
    Task<IEnumerable<Category>> GetAllWithChildrenAsync();
}
