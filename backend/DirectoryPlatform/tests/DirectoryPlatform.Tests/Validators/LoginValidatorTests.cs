using DirectoryPlatform.Application.Validators;
using DirectoryPlatform.Contracts.DTOs.Auth;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DirectoryPlatform.Tests.Validators;

public class LoginValidatorTests
{
    private readonly LoginValidator _validator = new();

    [Fact]
    public void ValidLogin_ShouldPass()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "password123" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyEmail_ShouldFail()
    {
        var dto = new LoginDto { Email = "", Password = "password123" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void InvalidEmailFormat_ShouldFail()
    {
        var dto = new LoginDto { Email = "not-an-email", Password = "password123" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void EmptyPassword_ShouldFail()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordTooShort_ShouldFail()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "12345" };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordExactlyMinLength_ShouldPass()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "123456" };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}
