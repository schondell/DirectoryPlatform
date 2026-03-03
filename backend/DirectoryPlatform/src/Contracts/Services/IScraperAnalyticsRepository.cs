using DirectoryPlatform.Contracts.DTOs.ScraperAnalytics;

namespace DirectoryPlatform.Contracts.Services;

public interface IScraperAnalyticsRepository
{
    Task<ScraperOverviewDto> GetOverviewAsync();
    Task<List<LifecycleEntryDto>> GetLifecycleAsync();
    Task<List<VelocityEntryDto>> GetVelocityAsync();
    Task<List<DealerEntryDto>> GetDealersAsync();
    Task<List<RepostEntryDto>> GetRepostsAsync();
    Task<List<PaidVsFreeEntryDto>> GetPaidVsFreeAsync();
    Task<List<PriceDistributionEntryDto>> GetPriceDistributionAsync();
    Task<List<GeoEntryDto>> GetGeographicAsync();
    Task<ScraperFreshnessDto> GetFreshnessAsync();
}
