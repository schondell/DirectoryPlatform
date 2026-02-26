namespace DirectoryPlatform.Contracts.DTOs.Dashboard;

public class UserKpiDashboardDto
{
    public KpiSummaryDto Summary { get; set; } = new();
    public List<KpiTimeSeriesDto> ViewsOverTime { get; set; } = new();
    public List<KpiTimeSeriesDto> LikesOverTime { get; set; } = new();
    public List<KpiTimeSeriesDto> MessagesOverTime { get; set; } = new();
    public List<KpiTimeSeriesDto> RevenueOverTime { get; set; } = new();
    public List<CategoryPerformanceDto> CategoryPerformance { get; set; } = new();
}

public class KpiSummaryDto
{
    public int TotalListings { get; set; }
    public int ActiveListings { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalFollowers { get; set; }
    public int TotalMessages { get; set; }
    public double AverageRating { get; set; }
    public double ResponseRate { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public decimal TotalRevenue { get; set; }
    public int ViewsTrend { get; set; }
    public int LikesTrend { get; set; }
    public int MessagesTrend { get; set; }
}

public class KpiTimeSeriesDto
{
    public DateTime Date { get; set; }
    public double Value { get; set; }
}

public class CategoryPerformanceDto
{
    public string CategoryName { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public double AverageRating { get; set; }
}
