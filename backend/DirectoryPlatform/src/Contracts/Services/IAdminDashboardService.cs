using DirectoryPlatform.Contracts.DTOs.Dashboard;

namespace DirectoryPlatform.Contracts.Services;

public interface IAdminDashboardService
{
    Task<AdminDashboardDto> GetDashboardAsync();
    Task<VisitorMetricsDto> GetVisitorMetricsAsync(int days = 30);
}
