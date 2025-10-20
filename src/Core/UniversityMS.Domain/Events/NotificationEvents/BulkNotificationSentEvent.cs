using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.NotificationEvents;

/// <summary>
/// Toplu bildirim gönderildiğinde tetiklenen event
/// </summary>
public class BulkNotificationSentEvent : BaseDomainEvent
{
    public Guid[] RecipientIds { get; }
    public NotificationType Type { get; }
    public int TotalRecipients { get; }
    public DateTime SentTime { get; }

    public BulkNotificationSentEvent(Guid[] recipientIds, NotificationType type, DateTime sentTime)
    {
        RecipientIds = recipientIds;
        Type = type;
        TotalRecipients = recipientIds.Length;
        SentTime = sentTime;
    }
}