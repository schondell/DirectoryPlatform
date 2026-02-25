using DirectoryPlatform.Contracts.DTOs.Message;
using FluentValidation;

namespace DirectoryPlatform.Application.Validators;

public class CreateMessageValidator : AbstractValidator<CreateMessageDto>
{
    public CreateMessageValidator()
    {
        RuleFor(x => x.RecipientId).NotEmpty();
        RuleFor(x => x.Subject).NotEmpty().MaximumLength(256);
        RuleFor(x => x.Body).NotEmpty();
    }
}
