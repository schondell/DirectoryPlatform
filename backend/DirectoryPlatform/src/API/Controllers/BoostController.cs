using DirectoryPlatform.Contracts.DTOs.Boost;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

[Authorize]
public class BoostController : BaseController
{
    private readonly IBoostService _boostService;

    public BoostController(IBoostService boostService)
    {
        _boostService = boostService;
    }

    [HttpGet("pricing")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<BoostPricingDto>>> GetPricing()
    {
        var pricing = await _boostService.GetPricingAsync();
        return Ok(pricing);
    }

    [HttpPost]
    public async Task<ActionResult<BoostDto>> CreateBoost([FromBody] CreateBoostDto dto)
    {
        var boost = await _boostService.CreateBoostAsync(dto, GetCurrentUserId());
        return Ok(boost);
    }

    [HttpGet("listing/{listingId}")]
    public async Task<ActionResult<IEnumerable<BoostDto>>> GetActiveBoosts(Guid listingId)
    {
        var boosts = await _boostService.GetActiveBoostsAsync(listingId);
        return Ok(boosts);
    }

    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<BoostDto>>> GetMyBoosts()
    {
        var boosts = await _boostService.GetUserBoostsAsync(GetCurrentUserId());
        return Ok(boosts);
    }
}
