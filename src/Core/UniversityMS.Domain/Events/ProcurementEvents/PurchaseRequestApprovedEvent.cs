using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ProcurementEvents;

public class PurchaseRequestApprovedEvent : BaseDomainEvent
{
    public Guid PurchaseRequestId { get; }
    public Guid ApproverId { get; }

    public PurchaseRequestApprovedEvent(Guid purchaseRequestId, Guid approverId)
    {
        PurchaseRequestId = purchaseRequestId;
        ApproverId = approverId;
    }
}