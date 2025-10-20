using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Yemekhane siparişi onaylandığında tetiklenen event
/// </summary>
public class CafeteriaOrderApprovedEvent : BaseDomainEvent
{
    public Guid OrderId { get; }
    public string OrderNumber { get; }
    public Guid ApprovedBy { get; }

    public CafeteriaOrderApprovedEvent(Guid orderId, string orderNumber, Guid approvedBy)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        ApprovedBy = approvedBy;
    }
}