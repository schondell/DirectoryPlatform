using DirectoryPlatform.Contracts.DTOs.Common;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

public class ListingsController : BaseController
{
    private readonly IListingService _listingService;

    public ListingsController(IListingService listingService)
    {
        _listingService = listingService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ListingDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] Guid? regionId = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool ascending = true)
    {
        // Extract attribute filters from query string (attr[key]=value)
        var attributes = new Dictionary<string, string>();
        foreach (var key in Request.Query.Keys)
        {
            if (key.StartsWith("attr[") && key.EndsWith("]"))
            {
                var attrSlug = key[5..^1];
                attributes[attrSlug] = Request.Query[key].ToString();
            }
        }

        var filter = new ListingFilterRequestDto
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SearchTerm = search,
            CategoryId = categoryId,
            RegionId = regionId,
            SortBy = sortBy,
            Ascending = ascending,
            Attributes = attributes
        };

        var result = await _listingService.GetFilteredListingsAsync(filter);
        return Ok(result);
    }

    [HttpGet("featured")]
    public async Task<ActionResult<IEnumerable<ListingDto>>> GetFeatured([FromQuery] int count = 10)
    {
        var listings = await _listingService.GetFeaturedAsync(count);
        return Ok(listings);
    }

    [HttpGet("recent")]
    public async Task<ActionResult<IEnumerable<ListingDto>>> GetRecent([FromQuery] int count = 10)
    {
        var listings = await _listingService.GetRecentAsync(count);
        return Ok(listings);
    }

    [Authorize]
    [HttpGet("my")]
    public async Task<ActionResult<IEnumerable<ListingDto>>> GetMyListings()
    {
        var listings = await _listingService.GetByUserIdAsync(GetCurrentUserId());
        return Ok(listings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ListingDto>> GetById(Guid id)
    {
        await _listingService.IncrementViewCountAsync(id);
        var listing = await _listingService.GetByIdAsync(id);
        return listing == null ? NotFound() : Ok(listing);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ListingDto>> Create([FromBody] CreateListingDto dto)
    {
        var listing = await _listingService.CreateAsync(dto, GetCurrentUserId());
        return CreatedAtAction(nameof(GetById), new { id = listing.Id }, listing);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<ListingDto>> Update(Guid id, [FromBody] UpdateListingDto dto)
    {
        try
        {
            var listing = await _listingService.UpdateAsync(id, dto, GetCurrentUserId());
            return Ok(listing);
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _listingService.DeleteAsync(id, GetCurrentUserId());
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (UnauthorizedAccessException) { return Forbid(); }
    }
}
