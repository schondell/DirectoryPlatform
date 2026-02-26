namespace DirectoryPlatform.Core.Entities;

public class VisitorMetric : BaseEntity
{
    public DateTime Date { get; set; }
    public int UniqueVisitors { get; set; }
    public int TotalPageViews { get; set; }
    public int NewUsers { get; set; }
    public int NewListings { get; set; }
}
