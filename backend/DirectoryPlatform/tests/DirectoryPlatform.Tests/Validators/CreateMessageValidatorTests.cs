using DirectoryPlatform.Application.Validators;
using DirectoryPlatform.Contracts.DTOs.Message;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DirectoryPlatform.Tests.Validators;

public class CreateMessageValidatorTests
{
    private readonly CreateMessageValidator _validator = new();

    [Fact]
    public void ValidMessage_ShouldPass()
    {
        var dto = new CreateMessageDto
        {
            RecipientId = Guid.NewGuid(),
            Subject = "Hello",
            Body = "Message body"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyRecipientId_ShouldFail()
    {
        var dto = new CreateMessageDto
        {
            RecipientId = Guid.Empty,
            Subject = "Hello",
            Body = "Body"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.RecipientId);
    }

    [Fact]
    public void EmptySubject_ShouldFail()
    {
        var dto = new CreateMessageDto
        {
            RecipientId = Guid.NewGuid(),
            Subject = "",
            Body = "Body"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Subject);
    }

    [Fact]
    public void SubjectExceedsMaxLength_ShouldFail()
    {
        var dto = new CreateMessageDto
        {
            RecipientId = Guid.NewGuid(),
            Subject = new string('a', 257),
            Body = "Body"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Subject);
    }

    [Fact]
    public void EmptyBody_ShouldFail()
    {
        var dto = new CreateMessageDto
        {
            RecipientId = Guid.NewGuid(),
            Subject = "Hello",
            Body = ""
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Body);
    }

    [Fact]
    public void SubjectAtMaxLength_ShouldPass()
    {
        var dto = new CreateMessageDto
        {
            RecipientId = Guid.NewGuid(),
            Subject = new string('a', 256),
            Body = "Body"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Subject);
    }
}
