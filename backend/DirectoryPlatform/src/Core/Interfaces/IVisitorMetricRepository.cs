using DirectoryPlatform.Core.Entities;

namespace DirectoryPlatform.Core.Interfaces;

public interface IVisitorMetricRepository : IRepository<VisitorMetric>
{
    Task<IEnumerable<VisitorMetric>> GetMetricsAsync(DateTime from, DateTime to);
    Task<VisitorMetric?> GetByDateAsync(DateTime date);
}
