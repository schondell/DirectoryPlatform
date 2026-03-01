using DirectoryPlatform.Application.Validators;
using DirectoryPlatform.Contracts.DTOs.Category;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DirectoryPlatform.Tests.Validators;

public class CreateCategoryValidatorTests
{
    private readonly CreateCategoryValidator _validator = new();

    [Fact]
    public void ValidCategory_ShouldPass()
    {
        var dto = new CreateCategoryDto { Name = "Electronics", Slug = "electronics" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyName_ShouldFail()
    {
        var dto = new CreateCategoryDto { Name = "", Slug = "electronics" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void EmptySlug_ShouldFail()
    {
        var dto = new CreateCategoryDto { Name = "Electronics", Slug = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Fact]
    public void NameExceedsMaxLength_ShouldFail()
    {
        var dto = new CreateCategoryDto { Name = new string('a', 201), Slug = "electronics" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void SlugExceedsMaxLength_ShouldFail()
    {
        var dto = new CreateCategoryDto { Name = "Electronics", Slug = new string('a', 201) };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Theory]
    [InlineData("Invalid Slug")]
    [InlineData("UPPERCASE")]
    [InlineData("special@chars")]
    [InlineData("has space")]
    public void InvalidSlugFormat_ShouldFail(string slug)
    {
        var dto = new CreateCategoryDto { Name = "Test", Slug = slug };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Slug);
    }

    [Theory]
    [InlineData("valid-slug")]
    [InlineData("123")]
    [InlineData("test-category-123")]
    public void ValidSlugFormat_ShouldPass(string slug)
    {
        var dto = new CreateCategoryDto { Name = "Test", Slug = slug };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Slug);
    }

    [Fact]
    public void NameAtMaxLength_ShouldPass()
    {
        var dto = new CreateCategoryDto { Name = new string('a', 200), Slug = "valid" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Name);
    }
}
