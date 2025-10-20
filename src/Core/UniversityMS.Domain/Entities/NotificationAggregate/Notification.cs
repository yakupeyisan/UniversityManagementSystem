using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.NotificationAggregate;


/// <summary>
/// Bildirim - Aggregate Root
/// </summary>
public class Notification : AuditableEntity, IAggregateRoot
{
    public string Title { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public NotificationType Type { get; private set; }
    public NotificationPriority Priority { get; private set; }
    public Guid RecipientId { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime SendDate { get; private set; }
    public DateTime? ReadDate { get; private set; }
    public string? DeliveryMethod { get; private set; }
    public bool DeliverySuccess { get; private set; }
    public string? ActionUrl { get; private set; }

    public Person Recipient { get; private set; } = null!;

    private Notification() { }

    private Notification(string title, string message, NotificationType type, NotificationPriority priority, Guid recipientId)
    {
        Title = title;
        Message = message;
        Type = type;
        Priority = priority;
        RecipientId = recipientId;
        Status = NotificationStatus.Created;
        SendDate = DateTime.UtcNow;
        DeliverySuccess = false;
    }

    public static Notification Create(string title, string message, NotificationType type, NotificationPriority priority, Guid recipientId)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(message))
            throw new DomainException("Başlık ve mesaj boş olamaz.");
        return new Notification(title, message, type, priority, recipientId);
    }

    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Read)
            throw new DomainException("Bildirim zaten okunmuş.");
        Status = NotificationStatus.Read;
        ReadDate = DateTime.UtcNow;
    }

    public void MarkAsDelivered(bool success)
    {
        Status = NotificationStatus.Sent;
        DeliverySuccess = success;
    }

    public void SetDeliveryMethod(string method) => DeliveryMethod = method;
    public void SetActionUrl(string url) => ActionUrl = url;
}