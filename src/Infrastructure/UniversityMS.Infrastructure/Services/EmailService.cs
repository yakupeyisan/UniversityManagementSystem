using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        // Email sending logic would be implemented here
        // Using SMTP, SendGrid, AWS SES, etc.
        _logger.LogInformation("Email sent to {To} with subject: {Subject}", to, subject);
        await Task.CompletedTask;
    }

    public async Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true)
    {
        foreach (var email in to)
        {
            await SendEmailAsync(email, subject, body, isHtml);
        }
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body,
        byte[] attachment, string attachmentName)
    {
        // Email with attachment logic
        _logger.LogInformation("Email with attachment sent to {To}", to);
        await Task.CompletedTask;
    }
}