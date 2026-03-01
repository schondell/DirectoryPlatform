using DirectoryPlatform.Application.Validators;
using DirectoryPlatform.Contracts.DTOs.Listing;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DirectoryPlatform.Tests.Validators;

public class CreateListingValidatorTests
{
    private readonly CreateListingValidator _validator = new();

    [Fact]
    public void ValidListing_ShouldPass()
    {
        var dto = new CreateListingDto
        {
            Title = "Test Listing",
            CategoryId = Guid.NewGuid()
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyTitle_ShouldFail()
    {
        var dto = new CreateListingDto { Title = "", CategoryId = Guid.NewGuid() };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void TitleExceedsMaxLength_ShouldFail()
    {
        var dto = new CreateListingDto { Title = new string('a', 257), CategoryId = Guid.NewGuid() };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void TitleAtMaxLength_ShouldPass()
    {
        var dto = new CreateListingDto { Title = new string('a', 256), CategoryId = Guid.NewGuid() };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void EmptyCategoryId_ShouldFail()
    {
        var dto = new CreateListingDto { Title = "Test", CategoryId = Guid.Empty };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.CategoryId);
    }

    [Fact]
    public void ShortDescriptionExceedsMaxLength_ShouldFail()
    {
        var dto = new CreateListingDto
        {
            Title = "Test",
            CategoryId = Guid.NewGuid(),
            ShortDescription = new string('a', 501)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ShortDescription);
    }

    [Fact]
    public void ShortDescriptionAtMaxLength_ShouldPass()
    {
        var dto = new CreateListingDto
        {
            Title = "Test",
            CategoryId = Guid.NewGuid(),
            ShortDescription = new string('a', 500)
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ShortDescription);
    }

    [Fact]
    public void NullShortDescription_ShouldPass()
    {
        var dto = new CreateListingDto
        {
            Title = "Test",
            CategoryId = Guid.NewGuid(),
            ShortDescription = null
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.ShortDescription);
    }
}
