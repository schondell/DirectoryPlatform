using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.Infrastructure.Repositories;

public class VisitorMetricRepository : Repository<VisitorMetric>, IVisitorMetricRepository
{
    public VisitorMetricRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<VisitorMetric>> GetMetricsAsync(DateTime from, DateTime to)
        => await _dbSet.Where(m => m.Date >= from && m.Date <= to).OrderBy(m => m.Date).ToListAsync();

    public async Task<VisitorMetric?> GetByDateAsync(DateTime date)
        => await _dbSet.FirstOrDefaultAsync(m => m.Date.Date == date.Date);
}
