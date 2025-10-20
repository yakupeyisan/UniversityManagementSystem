using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Yemekhane siparişi oluşturulduğunda tetiklenen event
/// </summary>
public class CafeteriaOrderCreatedEvent : BaseDomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Guid UserId { get; }

    public CafeteriaOrderCreatedEvent(Guid orderId, string orderNumber, Guid userId)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        UserId = userId;
    }
}