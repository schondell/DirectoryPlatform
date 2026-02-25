using DirectoryPlatform.Contracts.DTOs.Subscription;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

[Authorize]
public class SubscriptionsController : BaseController
{
    private readonly ISubscriptionService _subscriptionService;

    public SubscriptionsController(ISubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet("my")]
    public async Task<ActionResult<SubscriptionDto>> GetActive()
    {
        var sub = await _subscriptionService.GetActiveByUserIdAsync(GetCurrentUserId());
        return sub == null ? NotFound() : Ok(sub);
    }

    [AllowAnonymous]
    [HttpGet("tiers")]
    public async Task<ActionResult<IEnumerable<SubscriptionTierDto>>> GetTiers()
    {
        var tiers = await _subscriptionService.GetTiersAsync();
        return Ok(tiers);
    }

    [HttpPost]
    public async Task<ActionResult<SubscriptionDto>> Create([FromBody] CreateSubscriptionDto dto)
    {
        var sub = await _subscriptionService.CreateAsync(dto, GetCurrentUserId());
        return Ok(sub);
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            await _subscriptionService.CancelAsync(id, GetCurrentUserId());
            return Ok(new { message = "Subscription cancelled" });
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }
}
