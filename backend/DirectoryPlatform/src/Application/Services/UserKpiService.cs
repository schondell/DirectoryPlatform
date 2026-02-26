using DirectoryPlatform.Contracts.DTOs.Dashboard;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;

namespace DirectoryPlatform.Application.Services;

public class UserKpiService : IUserKpiService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IListingEngagementRepository _engagementRepo;
    private readonly IInvoiceRepository _invoiceRepo;

    public UserKpiService(IUnitOfWork unitOfWork, IListingEngagementRepository engagementRepo, IInvoiceRepository invoiceRepo)
    {
        _unitOfWork = unitOfWork;
        _engagementRepo = engagementRepo;
        _invoiceRepo = invoiceRepo;
    }

    public async Task<UserKpiDashboardDto> GetKpiDashboardAsync(Guid userId, int days = 30)
    {
        var summary = await GetSummaryAsync(userId);
        var from = DateTime.UtcNow.Date.AddDays(-days);
        var to = DateTime.UtcNow.Date;

        var userListings = await _unitOfWork.Listings.GetByUserIdAsync(userId);
        var listingIds = userListings.Select(l => l.Id).ToList();

        // Build time series for views
        var viewsTimeSeries = new List<KpiTimeSeriesDto>();
        foreach (var listingId in listingIds)
        {
            var views = await _engagementRepo.GetPageViewsAsync(listingId, from, to);
            foreach (var v in views)
            {
                var existing = viewsTimeSeries.FirstOrDefault(ts => ts.Date.Date == v.ViewDate.Date);
                if (existing != null) existing.Value += v.ViewCount;
                else viewsTimeSeries.Add(new KpiTimeSeriesDto { Date = v.ViewDate, Value = v.ViewCount });
            }
        }

        // Category performance
        var categoryPerformance = userListings
            .GroupBy(l => l.Category?.Name ?? "Uncategorized")
            .Select(g => new CategoryPerformanceDto
            {
                CategoryName = g.Key,
                ListingCount = g.Count(),
                TotalViews = g.Sum(l => l.ViewCount)
            }).ToList();

        return new UserKpiDashboardDto
        {
            Summary = summary,
            ViewsOverTime = viewsTimeSeries.OrderBy(v => v.Date).ToList(),
            CategoryPerformance = categoryPerformance
        };
    }

    public async Task<KpiSummaryDto> GetSummaryAsync(Guid userId)
    {
        var listings = await _unitOfWork.Listings.GetByUserIdAsync(userId);
        var listingsList = listings.ToList();

        var totalViews = listingsList.Sum(l => l.ViewCount);
        var totalLikes = 0;
        var totalFollowers = 0;

        foreach (var listing in listingsList)
        {
            totalLikes += await _engagementRepo.GetLikeCountAsync(listing.Id);
            totalFollowers += await _engagementRepo.GetFollowerCountAsync(listing.Id);
        }

        var receivedMessages = await _unitOfWork.Messages.GetInboxAsync(userId);
        var sentMessages = await _unitOfWork.Messages.GetSentAsync(userId);

        var reviews = new List<double>();
        foreach (var listing in listingsList)
        {
            var avg = await _unitOfWork.Reviews.GetAverageRatingAsync(listing.Id);
            if (avg > 0) reviews.Add(avg);
        }

        var totalMessages = receivedMessages.Count();
        var repliedMessages = sentMessages.Count();
        var responseRate = totalMessages > 0 ? (double)repliedMessages / totalMessages * 100 : 0;

        return new KpiSummaryDto
        {
            TotalListings = listingsList.Count,
            ActiveListings = listingsList.Count(l => l.Status == ListingStatus.Active),
            TotalViews = totalViews,
            TotalLikes = totalLikes,
            TotalFollowers = totalFollowers,
            TotalMessages = totalMessages,
            AverageRating = reviews.Count > 0 ? reviews.Average() : 0,
            ResponseRate = responseRate
        };
    }
}
