using DirectoryPlatform.Application.Validators;
using DirectoryPlatform.Contracts.DTOs.Auth;
using FluentAssertions;
using FluentValidation.TestHelper;

namespace DirectoryPlatform.Tests.Validators;

public class RegisterValidatorTests
{
    private readonly RegisterValidator _validator = new();

    [Fact]
    public void ValidRegistration_ShouldPass()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void EmptyUsername_ShouldFail()
    {
        var dto = new RegisterDto
        {
            Username = "",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void UsernameTooShort_ShouldFail()
    {
        var dto = new RegisterDto
        {
            Username = "ab",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void UsernameTooLong_ShouldFail()
    {
        var dto = new RegisterDto
        {
            Username = new string('a', 51),
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void InvalidEmail_ShouldFail()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "invalid-email",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void PasswordTooShort_ShouldFail()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "12345",
            ConfirmPassword = "12345"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordTooLong_ShouldFail()
    {
        var longPassword = new string('a', 101);
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = longPassword,
            ConfirmPassword = longPassword
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void PasswordMismatch_ShouldFail()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "differentpassword"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmPassword);
    }

    [Fact]
    public void EmptyPassword_ShouldFail()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "",
            ConfirmPassword = ""
        };
        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void UsernameAtMinLength_ShouldPass()
    {
        var dto = new RegisterDto
        {
            Username = "abc",
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    [Fact]
    public void UsernameAtMaxLength_ShouldPass()
    {
        var dto = new RegisterDto
        {
            Username = new string('a', 50),
            Email = "test@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }
}
