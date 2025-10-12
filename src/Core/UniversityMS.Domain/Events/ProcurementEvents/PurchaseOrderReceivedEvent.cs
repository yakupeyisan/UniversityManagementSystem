using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ProcurementEvents;

public class PurchaseOrderReceivedEvent : BaseDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string OrderNumber { get; }

    public PurchaseOrderReceivedEvent(Guid purchaseOrderId, string orderNumber)
    {
        PurchaseOrderId = purchaseOrderId;
        OrderNumber = orderNumber;
    }
}