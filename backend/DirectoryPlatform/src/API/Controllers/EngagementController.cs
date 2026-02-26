using DirectoryPlatform.Contracts.DTOs.Engagement;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

public class EngagementController : BaseController
{
    private readonly IListingEngagementService _engagementService;

    public EngagementController(IListingEngagementService engagementService)
    {
        _engagementService = engagementService;
    }

    [HttpGet("{listingId}")]
    public async Task<ActionResult<ListingEngagementDto>> GetEngagement(Guid listingId)
    {
        Guid? userId = User.Identity?.IsAuthenticated == true ? GetCurrentUserId() : null;
        var result = await _engagementService.GetEngagementAsync(listingId, userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("{listingId}/like")]
    public async Task<ActionResult<object>> ToggleLike(Guid listingId)
    {
        var isLiked = await _engagementService.ToggleLikeAsync(listingId, GetCurrentUserId());
        return Ok(new { isLiked });
    }

    [Authorize]
    [HttpPost("{listingId}/follow")]
    public async Task<ActionResult<object>> ToggleFollow(Guid listingId, [FromQuery] bool notifyOnUpdate = true)
    {
        var isFollowing = await _engagementService.ToggleFollowAsync(listingId, GetCurrentUserId(), notifyOnUpdate);
        return Ok(new { isFollowing });
    }

    [HttpPost("{listingId}/view")]
    public async Task<IActionResult> TrackView(Guid listingId)
    {
        await _engagementService.TrackPageViewAsync(listingId);
        if (User.Identity?.IsAuthenticated == true)
            await _engagementService.TrackVisitorAsync(listingId, GetCurrentUserId());
        return Ok();
    }

    [Authorize]
    [HttpGet("{listingId}/views")]
    public async Task<ActionResult<PageViewStatsDto>> GetPageViewStats(Guid listingId, [FromQuery] int days = 30)
    {
        var stats = await _engagementService.GetPageViewStatsAsync(listingId, days);
        return Ok(stats);
    }

    [Authorize]
    [HttpGet("{listingId}/visitors")]
    public async Task<ActionResult<VisitorStatsDto>> GetVisitorStats(Guid listingId, [FromQuery] int days = 30)
    {
        var stats = await _engagementService.GetVisitorStatsAsync(listingId, days);
        return Ok(stats);
    }

    [Authorize]
    [HttpGet("liked")]
    public async Task<ActionResult<IEnumerable<LikedListingDto>>> GetLikedListings()
    {
        var result = await _engagementService.GetLikedListingsAsync(GetCurrentUserId());
        return Ok(result);
    }

    [Authorize]
    [HttpGet("followed")]
    public async Task<ActionResult<IEnumerable<FollowedListingDto>>> GetFollowedListings()
    {
        var result = await _engagementService.GetFollowedListingsAsync(GetCurrentUserId());
        return Ok(result);
    }
}
