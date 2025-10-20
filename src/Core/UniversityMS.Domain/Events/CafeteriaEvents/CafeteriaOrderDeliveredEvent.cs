using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Yemekhane siparişi teslim edildiğinde tetiklenen event
/// </summary>
public class CafeteriaOrderDeliveredEvent : BaseDomainEvent
{
    public Guid OrderId { get; }
    public Guid UserId { get; }
    public DateTime DeliveryTime { get; }

    public CafeteriaOrderDeliveredEvent(Guid orderId, Guid userId, DateTime deliveryTime)
    {
        OrderId = orderId;
        UserId = userId;
        DeliveryTime = deliveryTime;
    }
}