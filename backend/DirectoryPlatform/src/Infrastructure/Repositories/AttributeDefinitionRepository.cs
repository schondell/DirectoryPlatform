using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class AttributeDefinitionRepository : Repository<AttributeDefinition>, IAttributeDefinitionRepository
{
    public AttributeDefinitionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<AttributeDefinition>> GetByCategoryIdAsync(Guid categoryId, bool filterableOnly = false)
    {
        var query = _dbSet.Where(a => a.CategoryId == categoryId);
        if (filterableOnly)
            query = query.Where(a => a.IsFilterable);
        return await query.OrderBy(a => a.DisplayOrder).ToListAsync();
    }
}
