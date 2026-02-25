using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IAttributeDefinitionRepository : IRepository<AttributeDefinition>
{
    Task<IEnumerable<AttributeDefinition>> GetByCategoryIdAsync(Guid categoryId, bool filterableOnly = false);
}
