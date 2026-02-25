using DirectoryPlatform.Contracts.DTOs.Listing;

namespace DirectoryPlatform.Contracts.Services;

public interface IAttributeDefinitionService
{
    Task<IEnumerable<AttributeDefinitionDto>> GetByCategoryIdAsync(Guid categoryId, bool filterableOnly = false);
    Task<AttributeDefinitionDto?> GetByIdAsync(Guid id);
    Task<AttributeDefinitionDto> CreateAsync(CreateAttributeDefinitionDto dto);
    Task<AttributeDefinitionDto> UpdateAsync(Guid id, UpdateAttributeDefinitionDto dto);
    Task DeleteAsync(Guid id);
}
