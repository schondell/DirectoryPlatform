using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Category>> GetRootCategoriesAsync()
        => await _dbSet.Where(c => c.ParentId == null).OrderBy(c => c.DisplayOrder).ToListAsync();

    public async Task<Category?> GetBySlugAsync(string slug)
        => await _dbSet.Include(c => c.Children).FirstOrDefaultAsync(c => c.Slug == slug);

    public async Task<Category?> GetWithChildrenAsync(Guid id)
        => await _dbSet.Include(c => c.Children).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Category>> GetAllWithChildrenAsync()
        => await _dbSet.Include(c => c.Children).OrderBy(c => c.DisplayOrder).ToListAsync();
}
