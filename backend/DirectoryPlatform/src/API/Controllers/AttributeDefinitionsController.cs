using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

public class AttributeDefinitionsController : BaseController
{
    private readonly IAttributeDefinitionService _service;

    public AttributeDefinitionsController(IAttributeDefinitionService service)
    {
        _service = service;
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<AttributeDefinitionDto>>> GetByCategory(
        Guid categoryId, [FromQuery] bool filterableOnly = false)
    {
        var attrs = await _service.GetByCategoryIdAsync(categoryId, filterableOnly);
        return Ok(attrs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AttributeDefinitionDto>> GetById(Guid id)
    {
        var attr = await _service.GetByIdAsync(id);
        return attr == null ? NotFound() : Ok(attr);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<ActionResult<AttributeDefinitionDto>> Create([FromBody] CreateAttributeDefinitionDto dto)
    {
        var attr = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = attr.Id }, attr);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<AttributeDefinitionDto>> Update(Guid id, [FromBody] UpdateAttributeDefinitionDto dto)
    {
        try
        {
            var attr = await _service.UpdateAsync(id, dto);
            return Ok(attr);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
