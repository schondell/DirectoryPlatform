using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers.Admin;

[Authorize(Roles = "Admin,SuperAdmin")]
[Route("api/admin/[controller]")]
public class ApprovalController : BaseController
{
    private readonly IListingService _listingService;

    public ApprovalController(IListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ListingDto>> UpdateStatus(
        Guid id, [FromBody] UpdateStatusRequest request)
    {
        try
        {
            var listing = await _listingService.UpdateStatusAsync(id, request.Status, GetCurrentUserId(), request.Comment);
            return Ok(listing);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
}
