using DirectoryPlatform.Contracts.DTOs.Review;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

public class ReviewsController : BaseController
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpGet("listing/{listingId}")]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByListing(Guid listingId)
    {
        var reviews = await _reviewService.GetByListingIdAsync(listingId);
        return Ok(reviews);
    }

    [HttpGet("listing/{listingId}/average")]
    public async Task<ActionResult<double>> GetAverage(Guid listingId)
    {
        var avg = await _reviewService.GetAverageRatingAsync(listingId);
        return Ok(avg);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ReviewDto>> Create([FromBody] CreateReviewDto dto)
    {
        var review = await _reviewService.CreateAsync(dto, GetCurrentUserId());
        return Ok(review);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ReviewDto>> UpdateStatus(Guid id, [FromBody] string status)
    {
        try
        {
            var review = await _reviewService.UpdateStatusAsync(id, status);
            return Ok(review);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _reviewService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
