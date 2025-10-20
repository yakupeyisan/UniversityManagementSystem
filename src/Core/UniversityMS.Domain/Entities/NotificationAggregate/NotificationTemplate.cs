using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.NotificationAggregate;

/// <summary>
/// Bildirim Şablonu
/// </summary>
public class NotificationTemplate : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public string Subject { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public bool IsActive { get; private set; }

    private NotificationTemplate() { }

    private NotificationTemplate(string code, string name, NotificationType type, string subject, string body)
    {
        Code = code;
        Name = name;
        Type = type;
        Subject = subject;
        Body = body;
        IsActive = true;
    }

    public static NotificationTemplate Create(string code, string name, NotificationType type, string subject, string body)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(subject))
            throw new DomainException("Kod, ad ve konu boş olamaz.");
        return new NotificationTemplate(code, name, type, subject, body);
    }

    public void Deactivate() => IsActive = false;
    public void UpdateContent(string subject, string body)
    {
        Subject = subject;
        Body = body;
    }
}