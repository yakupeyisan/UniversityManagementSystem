using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ProcurementEvents;

public class PurchaseRequestSubmittedEvent : BaseDomainEvent
{
    public Guid PurchaseRequestId { get; }
    public string RequestNumber { get; }

    public PurchaseRequestSubmittedEvent(Guid purchaseRequestId, string requestNumber)
    {
        PurchaseRequestId = purchaseRequestId;
        RequestNumber = requestNumber;
    }
}