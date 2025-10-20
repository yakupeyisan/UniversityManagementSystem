using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Yemekhane siparişi iptal edildiğinde tetiklenen event
/// </summary>
public class CafeteriaOrderCancelledEvent : BaseDomainEvent
{
    public Guid OrderId { get; }
    public string CancellationReason { get; }

    public CafeteriaOrderCancelledEvent(Guid orderId, string cancellationReason)
    {
        OrderId = orderId;
        CancellationReason = cancellationReason;
    }
}