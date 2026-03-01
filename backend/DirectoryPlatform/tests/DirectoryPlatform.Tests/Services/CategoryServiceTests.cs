using AutoMapper;
using DirectoryPlatform.Application.Mapping;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Category;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly IMapper _mapper;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _unitOfWorkMock.Setup(u => u.Categories).Returns(_categoryRepoMock.Object);

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
        _mapper = config.CreateMapper();

        _categoryService = new CategoryService(_unitOfWorkMock.Object, _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        var categories = new List<Category>
        {
            new() { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics" },
            new() { Id = Guid.NewGuid(), Name = "Vehicles", Slug = "vehicles" }
        };
        _categoryRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

        var result = await _categoryService.GetAllAsync();

        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Electronics");
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsCategory()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "Electronics", Slug = "electronics", Children = new List<Category>() };
        _categoryRepoMock.Setup(r => r.GetWithChildrenAsync(id)).ReturnsAsync(category);

        var result = await _categoryService.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Electronics");
        result.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        _categoryRepoMock.Setup(r => r.GetWithChildrenAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Category?)null);

        var result = await _categoryService.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetBySlugAsync_ExistingSlug_ReturnsCategory()
    {
        var category = new Category { Id = Guid.NewGuid(), Name = "Electronics", Slug = "electronics" };
        _categoryRepoMock.Setup(r => r.GetBySlugAsync("electronics")).ReturnsAsync(category);

        var result = await _categoryService.GetBySlugAsync("electronics");

        result.Should().NotBeNull();
        result!.Slug.Should().Be("electronics");
    }

    [Fact]
    public async Task GetBySlugAsync_NonExistingSlug_ReturnsNull()
    {
        _categoryRepoMock.Setup(r => r.GetBySlugAsync("nonexistent"))
            .ReturnsAsync((Category?)null);

        var result = await _categoryService.GetBySlugAsync("nonexistent");

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedCategory()
    {
        var dto = new CreateCategoryDto { Name = "New Category", Slug = "new-category", DisplayOrder = 1 };
        _categoryRepoMock.Setup(r => r.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => c);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _categoryService.CreateAsync(dto);

        result.Should().NotBeNull();
        result.Name.Should().Be("New Category");
        result.Slug.Should().Be("new-category");
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task UpdateAsync_ExistingCategory_ReturnsUpdated()
    {
        var id = Guid.NewGuid();
        var existing = new Category { Id = id, Name = "Old Name", Slug = "old-name" };
        var dto = new CreateCategoryDto { Name = "Updated Name", Slug = "updated-name" };

        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(existing);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _categoryService.UpdateAsync(id, dto);

        result.Name.Should().Be("Updated Name");
        result.Slug.Should().Be("updated-name");
    }

    [Fact]
    public async Task UpdateAsync_NonExisting_ThrowsKeyNotFoundException()
    {
        _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Category?)null);

        var act = () => _categoryService.UpdateAsync(Guid.NewGuid(), new CreateCategoryDto());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task DeleteAsync_ExistingCategory_Succeeds()
    {
        var id = Guid.NewGuid();
        var category = new Category { Id = id, Name = "ToDelete", Slug = "to-delete" };
        _categoryRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(category);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _categoryService.DeleteAsync(id);

        _categoryRepoMock.Verify(r => r.DeleteAsync(category), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExisting_ThrowsKeyNotFoundException()
    {
        _categoryRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Category?)null);

        var act = () => _categoryService.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task GetTreeAsync_ReturnsRootCategoriesOrdered()
    {
        var root1 = new Category { Id = Guid.NewGuid(), Name = "B", Slug = "b", DisplayOrder = 2, ParentId = null, Children = new List<Category>() };
        var root2 = new Category { Id = Guid.NewGuid(), Name = "A", Slug = "a", DisplayOrder = 1, ParentId = null, Children = new List<Category>() };
        var child = new Category { Id = Guid.NewGuid(), Name = "C", Slug = "c", DisplayOrder = 1, ParentId = root1.Id, Children = new List<Category>() };
        root1.Children.Add(child);

        _categoryRepoMock.Setup(r => r.GetAllWithChildrenAsync())
            .ReturnsAsync(new List<Category> { root1, root2, child });

        var result = (await _categoryService.GetTreeAsync()).ToList();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("A"); // DisplayOrder 1 first
        result[1].Name.Should().Be("B"); // DisplayOrder 2 second
    }
}
