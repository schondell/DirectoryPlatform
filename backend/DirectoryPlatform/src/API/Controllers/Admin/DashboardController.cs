using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DirectoryPlatform.API.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin/[controller]")]
public class DashboardController : BaseController
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetStats()
    {
        var stats = new
        {
            TotalUsers = await _context.Users.CountAsync(),
            TotalListings = await _context.Listings.CountAsync(),
            ActiveListings = await _context.Listings.CountAsync(l => l.Status == ListingStatus.Active),
            PendingListings = await _context.Listings.CountAsync(l => l.Status == ListingStatus.PendingApproval),
            TotalCategories = await _context.Categories.CountAsync(),
            TotalRegions = await _context.Regions.CountAsync(),
            TotalReviews = await _context.Reviews.CountAsync(),
            ActiveSubscriptions = await _context.Subscriptions.CountAsync(s => s.IsActive)
        };
        return Ok(stats);
    }
}
