using DirectoryPlatform.Contracts.DTOs.Dashboard;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

[Authorize]
public class KpiController : BaseController
{
    private readonly IUserKpiService _kpiService;

    public KpiController(IUserKpiService kpiService)
    {
        _kpiService = kpiService;
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<UserKpiDashboardDto>> GetDashboard([FromQuery] int days = 30)
    {
        var dashboard = await _kpiService.GetKpiDashboardAsync(GetCurrentUserId(), days);
        return Ok(dashboard);
    }

    [HttpGet("summary")]
    public async Task<ActionResult<KpiSummaryDto>> GetSummary()
    {
        var summary = await _kpiService.GetSummaryAsync(GetCurrentUserId());
        return Ok(summary);
    }
}
