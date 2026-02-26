namespace DirectoryPlatform.Contracts.DTOs.Dashboard;

public class AdminDashboardDto
{
    public OverviewMetricsDto Overview { get; set; } = new();
    public RecentActivityDto RecentActivity { get; set; } = new();
    public SystemHealthDto SystemHealth { get; set; } = new();
}

public class OverviewMetricsDto
{
    public int TotalUsers { get; set; }
    public int TotalListings { get; set; }
    public int ActiveListings { get; set; }
    public int PendingApprovals { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewListingsThisMonth { get; set; }
}

public class RecentActivityDto
{
    public List<RecentUserDto> RecentUsers { get; set; } = new();
    public List<RecentListingDto> RecentListings { get; set; } = new();
}

public class RecentUserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RecentListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class SystemHealthDto
{
    public double CpuUsagePercent { get; set; }
    public double MemoryUsageMB { get; set; }
    public double MemoryTotalMB { get; set; }
    public string DotNetVersion { get; set; } = string.Empty;
    public string OsPlatform { get; set; } = string.Empty;
    public TimeSpan Uptime { get; set; }
}

public class VisitorMetricsDto
{
    public int TotalVisitorsToday { get; set; }
    public int TotalPageViewsToday { get; set; }
    public List<DailyMetricDto> DailyMetrics { get; set; } = new();
}

public class DailyMetricDto
{
    public DateTime Date { get; set; }
    public int UniqueVisitors { get; set; }
    public int PageViews { get; set; }
    public int NewUsers { get; set; }
    public int NewListings { get; set; }
}
