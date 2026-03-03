using DirectoryPlatform.Contracts.DTOs.ScraperAnalytics;
using DirectoryPlatform.Contracts.Services;

namespace DirectoryPlatform.Application.Services;

public class ScraperAnalyticsService : IScraperAnalyticsService
{
    private readonly IScraperAnalyticsRepository _repository;

    public ScraperAnalyticsService(IScraperAnalyticsRepository repository)
    {
        _repository = repository;
    }

    public async Task<ScraperAnalyticsDashboardDto> GetFullDashboardAsync()
    {
        // Sequential calls — DbContext is not thread-safe
        var overview = await _repository.GetOverviewAsync();
        var lifecycle = await _repository.GetLifecycleAsync();
        var velocity = await _repository.GetVelocityAsync();
        var dealers = await _repository.GetDealersAsync();
        var reposts = await _repository.GetRepostsAsync();
        var paidVsFree = await _repository.GetPaidVsFreeAsync();
        var priceDistribution = await _repository.GetPriceDistributionAsync();
        var geographic = await _repository.GetGeographicAsync();
        var freshness = await _repository.GetFreshnessAsync();

        return new ScraperAnalyticsDashboardDto
        {
            Overview = overview,
            Lifecycle = lifecycle,
            Velocity = velocity,
            Dealers = dealers,
            Reposts = reposts,
            PaidVsFree = paidVsFree,
            PriceDistribution = priceDistribution,
            Geographic = geographic,
            Freshness = freshness
        };
    }

    public Task<ScraperOverviewDto> GetOverviewAsync() => _repository.GetOverviewAsync();
    public Task<List<LifecycleEntryDto>> GetLifecycleAsync() => _repository.GetLifecycleAsync();
    public Task<List<VelocityEntryDto>> GetVelocityAsync() => _repository.GetVelocityAsync();
    public Task<List<DealerEntryDto>> GetDealersAsync() => _repository.GetDealersAsync();
    public Task<List<RepostEntryDto>> GetRepostsAsync() => _repository.GetRepostsAsync();
    public Task<List<PaidVsFreeEntryDto>> GetPaidVsFreeAsync() => _repository.GetPaidVsFreeAsync();
    public Task<List<PriceDistributionEntryDto>> GetPriceDistributionAsync() => _repository.GetPriceDistributionAsync();
    public Task<List<GeoEntryDto>> GetGeographicAsync() => _repository.GetGeographicAsync();
    public Task<ScraperFreshnessDto> GetFreshnessAsync() => _repository.GetFreshnessAsync();
}
