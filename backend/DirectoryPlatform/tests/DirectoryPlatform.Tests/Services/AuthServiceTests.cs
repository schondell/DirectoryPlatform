using System.Linq.Expressions;
using DirectoryPlatform.Application.Services;
using DirectoryPlatform.Contracts.DTOs.Auth;
using DirectoryPlatform.Contracts.Services;
using DirectoryPlatform.Core.Entities;
using DirectoryPlatform.Core.Enums;
using DirectoryPlatform.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DirectoryPlatform.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();

        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);

        var configData = new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", "ThisIsAVeryLongSecretKeyForTestingPurposesOnly1234567890" },
            { "JwtSettings:Issuer", "TestIssuer" },
            { "JwtSettings:Audience", "TestAudience" },
            { "JwtSettings:ExpirationHours", "24" }
        };
        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configData)
            .Build();

        _authService = new AuthService(_unitOfWorkMock.Object, _configuration, _emailServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WithValidData_ReturnsAuthResponse()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(false);
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User u) => u);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _authService.RegisterAsync(dto);

        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Email.Should().Be("test@example.com");
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.Role.Should().Be("User");
    }

    [Fact]
    public async Task RegisterAsync_DuplicateEmail_ThrowsInvalidOperationException()
    {
        var dto = new RegisterDto
        {
            Username = "testuser",
            Email = "existing@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _userRepoMock.SetupSequence(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(true); // email exists

        var act = () => _authService.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Email already registered");
    }

    [Fact]
    public async Task RegisterAsync_DuplicateUsername_ThrowsInvalidOperationException()
    {
        var dto = new RegisterDto
        {
            Username = "existinguser",
            Email = "new@example.com",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        _userRepoMock.SetupSequence(r => r.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>()))
            .ReturnsAsync(false)  // email does not exist
            .ReturnsAsync(true);  // username exists

        var act = () => _authService.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Username already taken");
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_ThrowsUnauthorized()
    {
        var dto = new LoginDto { Email = "notfound@example.com", Password = "password" };

        _userRepoMock.Setup(r => r.GetByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null);

        var act = () => _authService.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact]
    public async Task LoginAsync_LockedAccount_ThrowsUnauthorized()
    {
        var user = CreateTestUser();
        user.IsLocked = true;
        user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);

        var dto = new LoginDto { Email = user.Email, Password = "password" };
        var act = () => _authService.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Account is locked");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_IncrementsFailedAttempts()
    {
        var user = CreateTestUser();
        user.FailedLoginAttempts = 0;

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new LoginDto { Email = user.Email, Password = "wrongpassword" };
        var act = () => _authService.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        user.FailedLoginAttempts.Should().Be(1);
    }

    [Fact]
    public async Task LoginAsync_FifthFailedAttempt_LocksAccount()
    {
        var user = CreateTestUser();
        user.FailedLoginAttempts = 4;

        _userRepoMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new LoginDto { Email = user.Email, Password = "wrongpassword" };
        var act = () => _authService.LoginAsync(dto);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
        user.IsLocked.Should().BeTrue();
        user.LockoutEnd.Should().NotBeNull();
    }

    [Fact]
    public async Task LogoutAsync_ValidUser_ClearsRefreshToken()
    {
        var user = CreateTestUser();
        user.RefreshToken = "some-token";
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        _userRepoMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        await _authService.LogoutAsync(user.Id);

        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiry.Should().BeNull();
    }

    [Fact]
    public async Task LogoutAsync_NonExistentUser_DoesNotThrow()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((User?)null);

        var act = () => _authService.LogoutAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task VerifyEmailAsync_ValidToken_ReturnsTrue()
    {
        var user = CreateTestUser();
        user.EmailVerificationToken = "valid-token";
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(1);

        _userRepoMock.Setup(r => r.GetByEmailVerificationTokenAsync("valid-token"))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _authService.VerifyEmailAsync("valid-token");

        result.Should().BeTrue();
        user.IsEmailVerified.Should().BeTrue();
        user.EmailVerificationToken.Should().BeNull();
    }

    [Fact]
    public async Task VerifyEmailAsync_InvalidToken_ReturnsFalse()
    {
        _userRepoMock.Setup(r => r.GetByEmailVerificationTokenAsync("invalid"))
            .ReturnsAsync((User?)null);

        var result = await _authService.VerifyEmailAsync("invalid");

        result.Should().BeFalse();
    }

    [Fact]
    public async Task VerifyEmailAsync_ExpiredToken_ReturnsFalse()
    {
        var user = CreateTestUser();
        user.EmailVerificationToken = "expired-token";
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddHours(-1);

        _userRepoMock.Setup(r => r.GetByEmailVerificationTokenAsync("expired-token"))
            .ReturnsAsync(user);

        var result = await _authService.VerifyEmailAsync("expired-token");

        result.Should().BeFalse();
    }

    private static User CreateTestUser()
    {
        // Generate proper PBKDF2 hash for "Password123!"
        var saltBytes = new byte[32];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);
        using var pbkdf2 = new System.Security.Cryptography.Rfc2898DeriveBytes(
            "Password123!", saltBytes, 100000, System.Security.Cryptography.HashAlgorithmName.SHA256);
        var hash = Convert.ToBase64String(pbkdf2.GetBytes(32));

        return new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = hash,
            PasswordSalt = salt,
            Role = UserRole.User,
            IsEmailVerified = true
        };
    }
}
