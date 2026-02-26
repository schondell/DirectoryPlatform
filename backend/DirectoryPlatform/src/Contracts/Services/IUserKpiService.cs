using DirectoryPlatform.Contracts.DTOs.Dashboard;

namespace DirectoryPlatform.Contracts.Services;

public interface IUserKpiService
{
    Task<UserKpiDashboardDto> GetKpiDashboardAsync(Guid userId, int days = 30);
    Task<KpiSummaryDto> GetSummaryAsync(Guid userId);
}
