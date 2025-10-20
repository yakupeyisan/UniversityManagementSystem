using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using System.Runtime;
using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Infrastructure.Configuration;

namespace UniversityMS.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }
    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        await SendEmailAsync(new[] { to }, subject, body, isHtml);
    }

    public async Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var message = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            foreach (var recipient in to)
            {
                message.To.Add(recipient);
            }

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", to));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", to));
            throw;
        }
        _logger.LogInformation("Email sent to {To} with subject: {Subject}", to, subject);
        await Task.CompletedTask;
    }


    public async Task<bool> SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath,
        CancellationToken cancellationToken)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
            {
                EnableSsl = _settings.EnableSsl,
                Credentials = new NetworkCredential(_settings.Username, _settings.Password)
            };

            using var message = new MailMessage(_settings.FromEmail, to)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            if (File.Exists(attachmentPath))
            {
                var attachment = new Attachment(attachmentPath);
                message.Attachments.Add(attachment);
            }

            await client.SendMailAsync(message);
            _logger.LogInformation("Email with attachment sent to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachment to {To}", to);
            throw;
        }
        _logger.LogInformation("Email with attachment sent to {To}", to);
        await Task.CompletedTask;
        return true;
    }
}