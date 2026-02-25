using DirectoryPlatform.Contracts.DTOs.Listing;
using FluentValidation;

namespace DirectoryPlatform.Application.Validators;

public class CreateListingValidator : AbstractValidator<CreateListingDto>
{
    public CreateListingValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
        RuleFor(x => x.ShortDescription).MaximumLength(500);
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
