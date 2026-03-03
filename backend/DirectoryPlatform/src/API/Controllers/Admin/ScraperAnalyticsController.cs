using DirectoryPlatform.Contracts.DTOs.ScraperAnalytics;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin/scraper-analytics")]
public class ScraperAnalyticsController : BaseController
{
    private readonly IScraperAnalyticsService _service;

    public ScraperAnalyticsController(IScraperAnalyticsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ScraperAnalyticsDashboardDto>> GetDashboard()
    {
        var dashboard = await _service.GetFullDashboardAsync();
        return Ok(dashboard);
    }

    [HttpGet("overview")]
    public async Task<ActionResult<ScraperOverviewDto>> GetOverview()
    {
        var result = await _service.GetOverviewAsync();
        return Ok(result);
    }

    [HttpGet("lifecycle")]
    public async Task<ActionResult<List<LifecycleEntryDto>>> GetLifecycle()
    {
        var result = await _service.GetLifecycleAsync();
        return Ok(result);
    }

    [HttpGet("velocity")]
    public async Task<ActionResult<List<VelocityEntryDto>>> GetVelocity()
    {
        var result = await _service.GetVelocityAsync();
        return Ok(result);
    }

    [HttpGet("dealers")]
    public async Task<ActionResult<List<DealerEntryDto>>> GetDealers()
    {
        var result = await _service.GetDealersAsync();
        return Ok(result);
    }

    [HttpGet("reposts")]
    public async Task<ActionResult<List<RepostEntryDto>>> GetReposts()
    {
        var result = await _service.GetRepostsAsync();
        return Ok(result);
    }

    [HttpGet("paid-vs-free")]
    public async Task<ActionResult<List<PaidVsFreeEntryDto>>> GetPaidVsFree()
    {
        var result = await _service.GetPaidVsFreeAsync();
        return Ok(result);
    }

    [HttpGet("price-distribution")]
    public async Task<ActionResult<List<PriceDistributionEntryDto>>> GetPriceDistribution()
    {
        var result = await _service.GetPriceDistributionAsync();
        return Ok(result);
    }

    [HttpGet("geographic")]
    public async Task<ActionResult<List<GeoEntryDto>>> GetGeographic()
    {
        var result = await _service.GetGeographicAsync();
        return Ok(result);
    }

    [HttpGet("freshness")]
    public async Task<ActionResult<ScraperFreshnessDto>> GetFreshness()
    {
        var result = await _service.GetFreshnessAsync();
        return Ok(result);
    }
}
