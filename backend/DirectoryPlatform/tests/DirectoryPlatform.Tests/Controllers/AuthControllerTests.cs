using System.Security.Claims;
using DirectoryPlatform.API.Controllers;
using DirectoryPlatform.Contracts.DTOs.Auth;
using DirectoryPlatform.Contracts.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DirectoryPlatform.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    private void SetupUser(Guid userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOk()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "password123" };
        var response = new AuthResponseDto
        {
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            Username = "testuser",
            Email = "user@example.com",
            Role = "User",
            UserId = Guid.NewGuid()
        };
        _authServiceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(response);

        var result = await _controller.Login(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        value.Token.Should().Be("jwt-token");
        value.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new LoginDto { Email = "user@example.com", Password = "wrong" };
        _authServiceMock.Setup(s => s.LoginAsync(dto))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid credentials"));

        var result = await _controller.Login(dto);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Register_ValidData_ReturnsOk()
    {
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "new@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        var response = new AuthResponseDto
        {
            Token = "jwt-token",
            RefreshToken = "refresh-token",
            Username = "newuser",
            Email = "new@example.com",
            Role = "User",
            UserId = Guid.NewGuid()
        };
        _authServiceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(response);

        var result = await _controller.Register(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        value.Username.Should().Be("newuser");
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        var dto = new RegisterDto
        {
            Username = "newuser",
            Email = "existing@example.com",
            Password = "password123",
            ConfirmPassword = "password123"
        };
        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ThrowsAsync(new InvalidOperationException("Email already registered"));

        var result = await _controller.Register(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsOk()
    {
        var dto = new RefreshTokenDto { Token = "old-jwt", RefreshToken = "old-refresh" };
        var response = new AuthResponseDto
        {
            Token = "new-jwt",
            RefreshToken = "new-refresh",
            Username = "testuser",
            Email = "test@example.com",
            Role = "User"
        };
        _authServiceMock.Setup(s => s.RefreshTokenAsync(dto)).ReturnsAsync(response);

        var result = await _controller.RefreshToken(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var value = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;
        value.Token.Should().Be("new-jwt");
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_ReturnsUnauthorized()
    {
        var dto = new RefreshTokenDto { Token = "invalid", RefreshToken = "invalid" };
        _authServiceMock.Setup(s => s.RefreshTokenAsync(dto))
            .ThrowsAsync(new UnauthorizedAccessException("Invalid refresh token"));

        var result = await _controller.RefreshToken(dto);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Logout_AuthenticatedUser_ReturnsOk()
    {
        var userId = Guid.NewGuid();
        SetupUser(userId);

        var result = await _controller.Logout();

        result.Should().BeOfType<OkObjectResult>();
        _authServiceMock.Verify(s => s.LogoutAsync(userId), Times.Once);
    }

    [Fact]
    public async Task VerifyEmail_ValidToken_ReturnsOk()
    {
        _authServiceMock.Setup(s => s.VerifyEmailAsync("valid-token")).ReturnsAsync(true);

        var result = await _controller.VerifyEmail("valid-token");

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task VerifyEmail_InvalidToken_ReturnsBadRequest()
    {
        _authServiceMock.Setup(s => s.VerifyEmailAsync("invalid-token")).ReturnsAsync(false);

        var result = await _controller.VerifyEmail("invalid-token");

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ForgotPassword_AlwaysReturnsOk()
    {
        var dto = new ForgotPasswordDto { Email = "any@example.com" };

        var result = await _controller.ForgotPassword(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ResetPassword_ValidToken_ReturnsOk()
    {
        var dto = new ResetPasswordDto { Token = "valid", Password = "newpassword", ConfirmPassword = "newpassword" };

        var result = await _controller.ResetPassword(dto);

        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
    {
        var dto = new ResetPasswordDto { Token = "invalid", Password = "newpassword", ConfirmPassword = "newpassword" };
        _authServiceMock.Setup(s => s.ResetPasswordAsync(dto))
            .ThrowsAsync(new InvalidOperationException("Invalid reset token"));

        var result = await _controller.ResetPassword(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
