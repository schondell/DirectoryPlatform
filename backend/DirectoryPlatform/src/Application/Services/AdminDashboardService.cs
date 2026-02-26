using System.Diagnostics;
using System.Runtime.InteropServices;
using DirectoryPlatform.Contracts.DTOs.Dashboard;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInvoiceRepository _invoiceRepo;
    private readonly IVisitorMetricRepository _visitorMetricRepo;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public AdminDashboardService(IUnitOfWork unitOfWork, IInvoiceRepository invoiceRepo, IVisitorMetricRepository visitorMetricRepo)
    {
        _unitOfWork = unitOfWork;
        _invoiceRepo = invoiceRepo;
        _visitorMetricRepo = visitorMetricRepo;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var totalUsers = await _unitOfWork.Users.CountAsync();
        var totalListings = await _unitOfWork.Listings.CountAsync();
        var activeListings = await _unitOfWork.Listings.CountAsync(l => l.Status == ListingStatus.Active);
        var pendingApprovals = await _unitOfWork.Listings.CountAsync(l => l.Status == ListingStatus.PendingApproval);
        var monthlyRevenue = await _invoiceRepo.GetTotalRevenueAsync(monthStart, now);
        var newUsersThisMonth = await _unitOfWork.Users.CountAsync(u => u.CreatedAt >= monthStart);
        var newListingsThisMonth = await _unitOfWork.Listings.CountAsync(l => l.CreatedAt >= monthStart);

        var recentUsers = (await _unitOfWork.Users.FindAsync(u => true))
            .OrderByDescending(u => u.CreatedAt).Take(10)
            .Select(u => new RecentUserDto { Id = u.Id, Username = u.Username, Email = u.Email, CreatedAt = u.CreatedAt });

        var recentListings = (await _unitOfWork.Listings.GetRecentAsync(10))
            .Select(l => new RecentListingDto { Id = l.Id, Title = l.Title, CategoryName = l.Category?.Name, Status = l.Status.ToString(), CreatedAt = l.CreatedAt });

        var process = Process.GetCurrentProcess();

        return new AdminDashboardDto
        {
            Overview = new OverviewMetricsDto
            {
                TotalUsers = totalUsers,
                TotalListings = totalListings,
                ActiveListings = activeListings,
                PendingApprovals = pendingApprovals,
                MonthlyRevenue = monthlyRevenue,
                NewUsersThisMonth = newUsersThisMonth,
                NewListingsThisMonth = newListingsThisMonth
            },
            RecentActivity = new RecentActivityDto
            {
                RecentUsers = recentUsers.ToList(),
                RecentListings = recentListings.ToList()
            },
            SystemHealth = new SystemHealthDto
            {
                MemoryUsageMB = process.WorkingSet64 / 1024.0 / 1024.0,
                MemoryTotalMB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024.0 / 1024.0,
                DotNetVersion = RuntimeInformation.FrameworkDescription,
                OsPlatform = RuntimeInformation.OSDescription,
                Uptime = DateTime.UtcNow - _startTime
            }
        };
    }

    public async Task<VisitorMetricsDto> GetVisitorMetricsAsync(int days = 30)
    {
        var from = DateTime.UtcNow.Date.AddDays(-days);
        var to = DateTime.UtcNow.Date;
        var metrics = await _visitorMetricRepo.GetMetricsAsync(from, to);
        var today = metrics.FirstOrDefault(m => m.Date.Date == DateTime.UtcNow.Date);

        return new VisitorMetricsDto
        {
            TotalVisitorsToday = today?.UniqueVisitors ?? 0,
            TotalPageViewsToday = today?.TotalPageViews ?? 0,
            DailyMetrics = metrics.Select(m => new DailyMetricDto
            {
                Date = m.Date,
                UniqueVisitors = m.UniqueVisitors,
                PageViews = m.TotalPageViews,
                NewUsers = m.NewUsers,
                NewListings = m.NewListings
            }).ToList()
        };
    }
}
