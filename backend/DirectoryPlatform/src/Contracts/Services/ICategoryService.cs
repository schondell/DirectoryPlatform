using DirectoryPlatform.Contracts.DTOs.Category;

namespace DirectoryPlatform.Contracts.Services;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<IEnumerable<CategoryWithChildrenDto>> GetTreeAsync();
    Task<CategoryWithChildrenDto?> GetByIdAsync(Guid id);
    Task<CategoryDto?> GetBySlugAsync(string slug);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<CategoryDto> UpdateAsync(Guid id, CreateCategoryDto dto);
    Task DeleteAsync(Guid id);
}
