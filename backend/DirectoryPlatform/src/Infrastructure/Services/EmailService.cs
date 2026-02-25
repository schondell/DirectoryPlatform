using DirectoryPlatform.Contracts.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace DirectoryPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["EmailSettings:SenderName"] ?? "DirectoryPlatform",
            _configuration["EmailSettings:SenderEmail"] ?? "noreply@directoryplatform.com"));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration["EmailSettings:SmtpHost"] ?? "localhost",
            int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587"),
            MailKit.Security.SecureSocketOptions.StartTls);

        var smtpUser = _configuration["EmailSettings:SmtpUser"];
        if (!string.IsNullOrEmpty(smtpUser))
        {
            await client.AuthenticateAsync(smtpUser, _configuration["EmailSettings:SmtpPass"]);
        }

        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendVerificationEmailAsync(string to, string token)
    {
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:4200";
        var body = $"<h2>Verify Your Email</h2><p>Click <a href='{baseUrl}/auth/verify-email?token={token}'>here</a> to verify your email address.</p>";
        await SendEmailAsync(to, "Verify Your Email - DirectoryPlatform", body);
    }

    public async Task SendPasswordResetEmailAsync(string to, string token)
    {
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "http://localhost:4200";
        var body = $"<h2>Reset Your Password</h2><p>Click <a href='{baseUrl}/auth/reset-password?token={token}'>here</a> to reset your password.</p>";
        await SendEmailAsync(to, "Reset Your Password - DirectoryPlatform", body);
    }
}
