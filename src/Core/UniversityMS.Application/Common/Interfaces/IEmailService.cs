namespace UniversityMS.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body,
        byte[] attachment, string attachmentName);
    Task<bool> SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath, CancellationToken cancellationToken);

}