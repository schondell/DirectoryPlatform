using DirectoryPlatform.Contracts.DTOs.Category;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryPlatform.API.Controllers;

public class CategoriesController : BaseController
{
    private readonly ICategoryService _categoryService;
    private readonly IAttributeDefinitionService _attributeService;

    public CategoriesController(ICategoryService categoryService, IAttributeDefinitionService attributeService)
    {
        _categoryService = categoryService;
        _attributeService = attributeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(categories);
    }

    [HttpGet("tree")]
    public async Task<ActionResult<IEnumerable<CategoryWithChildrenDto>>> GetTree()
    {
        var tree = await _categoryService.GetTreeAsync();
        return Ok(tree);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryWithChildrenDto>> GetById(Guid id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoryDto>> GetBySlug(string slug)
    {
        var category = await _categoryService.GetBySlugAsync(slug);
        return category == null ? NotFound() : Ok(category);
    }

    [HttpGet("{id}/attributes")]
    public async Task<ActionResult<IEnumerable<AttributeDefinitionDto>>> GetAttributes(
        Guid id, [FromQuery] bool filterableOnly = false)
    {
        var attrs = await _attributeService.GetByCategoryIdAsync(id, filterableOnly);
        return Ok(attrs);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpPut("{id}")]
    public async Task<ActionResult<CategoryDto>> Update(Guid id, [FromBody] CreateCategoryDto dto)
    {
        try
        {
            var category = await _categoryService.UpdateAsync(id, dto);
            return Ok(category);
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException) { return NotFound(); }
    }
}
