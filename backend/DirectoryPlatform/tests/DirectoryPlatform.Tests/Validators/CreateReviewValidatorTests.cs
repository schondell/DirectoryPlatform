using DirectoryPlatform.Application.Validators;
using DirectoryPlatform.Contracts.DTOs.Review;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DirectoryPlatform.Tests.Validators;

public class CreateReviewValidatorTests
{
    private readonly CreateReviewValidator _validator = new();

    [Fact]
    public void ValidReview_ShouldPass()
    {
        var dto = new CreateReviewDto { ListingId = Guid.NewGuid(), Rating = 4, Comment = "Great!" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyListingId_ShouldFail()
    {
        var dto = new CreateReviewDto { ListingId = Guid.Empty, Rating = 3 };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ListingId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(6)]
    [InlineData(100)]
    public void InvalidRating_ShouldFail(int rating)
    {
        var dto = new CreateReviewDto { ListingId = Guid.NewGuid(), Rating = rating };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Rating);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void ValidRating_ShouldPass(int rating)
    {
        var dto = new CreateReviewDto { ListingId = Guid.NewGuid(), Rating = rating };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Rating);
    }

    [Fact]
    public void NullComment_ShouldPass()
    {
        var dto = new CreateReviewDto { ListingId = Guid.NewGuid(), Rating = 3, Comment = null };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
