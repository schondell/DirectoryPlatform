using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class RegionRepository : Repository<Region>, IRegionRepository
{
    public RegionRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Region>> GetRootRegionsAsync()
        => await _dbSet.Where(r => r.ParentId == null).OrderBy(r => r.DisplayOrder).ToListAsync();

    public async Task<Region?> GetBySlugAsync(string slug)
        => await _dbSet.Include(r => r.Children).FirstOrDefaultAsync(r => r.Slug == slug);

    public async Task<Region?> GetWithChildrenAsync(Guid id)
        => await _dbSet.Include(r => r.Children).FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Region>> GetAllWithChildrenAsync()
        => await _dbSet.Include(r => r.Children).OrderBy(r => r.DisplayOrder).ToListAsync();
}
