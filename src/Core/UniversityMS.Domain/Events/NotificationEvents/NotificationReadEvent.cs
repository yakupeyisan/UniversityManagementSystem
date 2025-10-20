using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.NotificationEvents;

/// <summary>
/// Bildirim okunduğunda tetiklenen event
/// </summary>
public class NotificationReadEvent : BaseDomainEvent
{
    public Guid NotificationId { get; }
    public Guid RecipientId { get; }
    public DateTime ReadTime { get; }

    public NotificationReadEvent(Guid notificationId, Guid recipientId, DateTime readTime)
    {
        NotificationId = notificationId;
        RecipientId = recipientId;
        ReadTime = readTime;
    }
}