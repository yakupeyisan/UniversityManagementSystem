using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.DocumentEvents;

/// <summary>
/// Belge teslim edildiğinde tetiklenen event
/// </summary>
public class DocumentDeliveredEvent : BaseDomainEvent
{
    public Guid DocumentId { get; }
    public Guid UserId { get; }
    public DateTime DeliveryDate { get; }

    public DocumentDeliveredEvent(Guid documentId, Guid userId, DateTime deliveryDate)
    {
        DocumentId = documentId;
        UserId = userId;
        DeliveryDate = deliveryDate;
    }
}