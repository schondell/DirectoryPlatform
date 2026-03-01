using System.Security.Claims;
using DirectoryPlatform.API.Controllers;
using DirectoryPlatform.Contracts.DTOs.Category;
using DirectoryPlatform.Contracts.DTOs.Listing;
using DirectoryPlatform.Contracts.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DirectoryPlatform.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _categoryServiceMock;
    private readonly Mock<IAttributeDefinitionService> _attrServiceMock;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _categoryServiceMock = new Mock<ICategoryService>();
        _attrServiceMock = new Mock<IAttributeDefinitionService>();
        _controller = new CategoriesController(_categoryServiceMock.Object, _attrServiceMock.Object);

        // Set up a default user context
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithCategories()
    {
        var categories = new List<CategoryDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics" },
            new() { Id = Guid.NewGuid(), Name = "Vehicles", Slug = "vehicles" }
        };
        _categoryServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(categories);

        var result = await _controller.GetAll();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as IEnumerable<CategoryDto>;
        value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTree_ReturnsOkWithTree()
    {
        var tree = new List<CategoryWithChildrenDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Root", Slug = "root", Children = new List<CategoryWithChildrenDto>() }
        };
        _categoryServiceMock.Setup(s => s.GetTreeAsync()).ReturnsAsync(tree);

        var result = await _controller.GetTree();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as IEnumerable<CategoryWithChildrenDto>;
        value.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_Existing_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var category = new CategoryWithChildrenDto { Id = id, Name = "Test", Slug = "test", Children = new List<CategoryWithChildrenDto>() };
        _categoryServiceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(category);

        var result = await _controller.GetById(id);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<CategoryWithChildrenDto>().Subject;
        value.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_NonExisting_ReturnsNotFound()
    {
        _categoryServiceMock.Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((CategoryWithChildrenDto?)null);

        var result = await _controller.GetById(Guid.NewGuid());

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetBySlug_Existing_ReturnsOk()
    {
        var category = new CategoryDto { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics" };
        _categoryServiceMock.Setup(s => s.GetBySlugAsync("electronics")).ReturnsAsync(category);

        var result = await _controller.GetBySlug("electronics");

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<CategoryDto>().Subject;
        value.Slug.Should().Be("electronics");
    }

    [Fact]
    public async Task GetBySlug_NonExisting_ReturnsNotFound()
    {
        _categoryServiceMock.Setup(s => s.GetBySlugAsync("nonexistent"))
            .ReturnsAsync((CategoryDto?)null);

        var result = await _controller.GetBySlug("nonexistent");

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetAttributes_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var attrs = new List<AttributeDefinitionDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Brand", Slug = "brand" }
        };
        _attrServiceMock.Setup(s => s.GetByCategoryIdAsync(id, false)).ReturnsAsync(attrs);

        var result = await _controller.GetAttributes(id);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value as IEnumerable<AttributeDefinitionDto>;
        value.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_ValidData_ReturnsCreatedAtAction()
    {
        var dto = new CreateCategoryDto { Name = "New Category", Slug = "new-category" };
        var created = new CategoryDto { Id = Guid.NewGuid(), Name = "New Category", Slug = "new-category" };
        _categoryServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetById");
        var value = createdResult.Value.Should().BeOfType<CategoryDto>().Subject;
        value.Name.Should().Be("New Category");
    }

    [Fact]
    public async Task Update_Existing_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new CreateCategoryDto { Name = "Updated", Slug = "updated" };
        var updated = new CategoryDto { Id = id, Name = "Updated", Slug = "updated" };
        _categoryServiceMock.Setup(s => s.UpdateAsync(id, dto)).ReturnsAsync(updated);

        var result = await _controller.Update(id, dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<CategoryDto>().Subject;
        value.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task Update_NonExisting_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new CreateCategoryDto { Name = "Updated" };
        _categoryServiceMock.Setup(s => s.UpdateAsync(id, dto))
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.Update(id, dto);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_Existing_ReturnsNoContent()
    {
        var id = Guid.NewGuid();

        var result = await _controller.Delete(id);

        result.Should().BeOfType<NoContentResult>();
        _categoryServiceMock.Verify(s => s.DeleteAsync(id), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExisting_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        _categoryServiceMock.Setup(s => s.DeleteAsync(id))
            .ThrowsAsync(new KeyNotFoundException());

        var result = await _controller.Delete(id);

        result.Should().BeOfType<NotFoundResult>();
    }
}
