using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.NotificationEvents;

/// <summary>
/// Bildirim gönderimi başarısız olduğunda tetiklenen event
/// </summary>
public class NotificationFailedEvent : BaseDomainEvent
{
    public Guid NotificationId { get; }
    public string DeliveryMethod { get; }
    public string ErrorMessage { get; }

    public NotificationFailedEvent(Guid notificationId, string deliveryMethod, string errorMessage)
    {
        NotificationId = notificationId;
        DeliveryMethod = deliveryMethod;
        ErrorMessage = errorMessage;
    }
}