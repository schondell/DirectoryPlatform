using DirectoryPlatform.Contracts.DTOs.Region;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

public class RegionsController : BaseController
{
    private readonly IRegionService _regionService;

    public RegionsController(IRegionService regionService)
    {
        _regionService = regionService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegionDto>>> GetAll()
    {
        var regions = await _regionService.GetAllAsync();
        return Ok(regions);
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<RegionWithChildrenDto>>> GetTree()
    {
        var tree = await _regionService.GetTreeAsync();
        return Ok(tree);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RegionWithChildrenDto>> GetById(Guid id)
    {
        var region = await _regionService.GetByIdAsync(id);
        return region == null ? NotFound() : Ok(region);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<RegionDto>> GetBySlug(string slug)
    {
        var region = await _regionService.GetBySlugAsync(slug);
        return region == null ? NotFound() : Ok(region);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<ActionResult<RegionDto>> Create([FromBody] CreateRegionDto dto)
    {
        var region = await _regionService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = region.Id }, region);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<RegionDto>> Update(Guid id, [FromBody] CreateRegionDto dto)
    {
        try
        {
            var region = await _regionService.UpdateAsync(id, dto);
            return Ok(region);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _regionService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
