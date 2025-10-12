using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ProcurementEvents;

public class PurchaseOrderApprovedEvent : BaseDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string OrderNumber { get; }
    public Guid SupplierId { get; }

    public PurchaseOrderApprovedEvent(Guid purchaseOrderId, string orderNumber, Guid supplierId)
    {
        PurchaseOrderId = purchaseOrderId;
        OrderNumber = orderNumber;
        SupplierId = supplierId;
    }
}