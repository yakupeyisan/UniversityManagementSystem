using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.NotificationEvents;

/// <summary>
/// Bildirim gönderildiğinde tetiklenen event
/// </summary>
public class NotificationSentEvent : BaseDomainEvent
{
    public Guid NotificationId { get; }
    public string DeliveryMethod { get; }
    public DateTime SentTime { get; }

    public NotificationSentEvent(Guid notificationId, string deliveryMethod, DateTime sentTime)
    {
        NotificationId = notificationId;
        DeliveryMethod = deliveryMethod;
        SentTime = sentTime;
    }
}