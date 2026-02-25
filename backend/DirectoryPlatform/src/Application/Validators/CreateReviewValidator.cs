using DirectoryPlatform.Contracts.DTOs.Review;
using FluentValidation;

namespace DirectoryPlatform.Application.Validators;

public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.ListingId).NotEmpty();
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
    }
}
