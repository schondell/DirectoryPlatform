using DirectoryPlatform.Contracts.DTOs.Dashboard;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin/[controller]")]
public class AdminDashboardController : BaseController
{
    private readonly IAdminDashboardService _dashboardService;

    public AdminDashboardController(IAdminDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<AdminDashboardDto>> GetDashboard()
    {
        var dashboard = await _dashboardService.GetDashboardAsync();
        return Ok(dashboard);
    }

    [HttpGet("visitors")]
    public async Task<ActionResult<VisitorMetricsDto>> GetVisitorMetrics([FromQuery] int days = 30)
    {
        var metrics = await _dashboardService.GetVisitorMetricsAsync(days);
        return Ok(metrics);
    }
}
