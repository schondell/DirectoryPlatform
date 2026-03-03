namespace DirectoryPlatform.Contracts.DTOs.ScraperAnalytics;

public class ScraperAnalyticsDashboardDto
{
    public ScraperOverviewDto Overview { get; set; } = new();
    public List<LifecycleEntryDto> Lifecycle { get; set; } = [];
    public List<VelocityEntryDto> Velocity { get; set; } = [];
    public List<DealerEntryDto> Dealers { get; set; } = [];
    public List<RepostEntryDto> Reposts { get; set; } = [];
    public List<PaidVsFreeEntryDto> PaidVsFree { get; set; } = [];
    public List<PriceDistributionEntryDto> PriceDistribution { get; set; } = [];
    public List<GeoEntryDto> Geographic { get; set; } = [];
    public ScraperFreshnessDto Freshness { get; set; } = new();
}

public class ScraperOverviewDto
{
    public int Total { get; set; }
    public int Active { get; set; }
    public int Expired { get; set; }
    public int Paid { get; set; }
    public int WithPhone { get; set; }
    public int WithImages { get; set; }
    public int UniquePhones { get; set; }
    public int Categories { get; set; }
    public DateTime? OldestListing { get; set; }
    public DateTime? NewestListing { get; set; }
}

public class LifecycleEntryDto
{
    public string Category { get; set; } = string.Empty;
    public int ExpiredCount { get; set; }
    public double AvgDaysAlive { get; set; }
    public double MinDays { get; set; }
    public double MaxDays { get; set; }
    public double MedianDays { get; set; }
}

public class VelocityEntryDto
{
    public string Category { get; set; } = string.Empty;
    public int TotalListings { get; set; }
    public int Last7d { get; set; }
    public int Last24h { get; set; }
    public double AvgPerDay { get; set; }
}

public class DealerEntryDto
{
    public string Phone { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public int CategoryCount { get; set; }
    public int PaidCount { get; set; }
    public int ActiveCount { get; set; }
    public List<string> Pseudos { get; set; } = [];
    public List<string> Locations { get; set; } = [];
}

public class RepostEntryDto
{
    public string Phone { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int RepostCount { get; set; }
    public double? AvgHoursBetween { get; set; }
    public string? SampleTitle { get; set; }
}

public class PaidVsFreeEntryDto
{
    public bool IsPaid { get; set; }
    public string? PaidType { get; set; }
    public int Count { get; set; }
    public int Active { get; set; }
    public int Expired { get; set; }
    public double? AvgDaysAlive { get; set; }
    public int WithPhone { get; set; }
    public double AvgImages { get; set; }
}

public class PriceDistributionEntryDto
{
    public string Category { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Avg { get; set; }
    public decimal Median { get; set; }
    public decimal Min { get; set; }
    public decimal Max { get; set; }
}

public class GeoEntryDto
{
    public string Location { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public int Active { get; set; }
    public int UniqueSellers { get; set; }
}

public class ScraperFreshnessDto
{
    public int TotalActive { get; set; }
    public int Seen24h { get; set; }
    public int Seen7d { get; set; }
    public int Seen30d { get; set; }
    public int Stale30d { get; set; }
    public double AvgDaysSinceSeen { get; set; }
}
