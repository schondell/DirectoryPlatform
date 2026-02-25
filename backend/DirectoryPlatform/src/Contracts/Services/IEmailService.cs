namespace DirectoryPlatform.Contracts.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendVerificationEmailAsync(string to, string token);
    Task SendPasswordResetEmailAsync(string to, string token);
}
