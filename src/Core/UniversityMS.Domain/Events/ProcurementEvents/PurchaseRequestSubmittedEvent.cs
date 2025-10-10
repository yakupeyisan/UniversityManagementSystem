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

public class PurchaseOrderCreatedEvent : BaseDomainEvent
{
    public Guid PurchaseOrderId { get; }
    public string OrderNumber { get; }
    public Guid SupplierId { get; }

    public PurchaseOrderCreatedEvent(Guid purchaseOrderId, string orderNumber, Guid supplierId)
    {
        PurchaseOrderId = purchaseOrderId;
        OrderNumber = orderNumber;
        SupplierId = supplierId;
    }
}

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

public class SupplierAddedEvent : BaseDomainEvent
{
    public Guid SupplierId { get; }
    public string Code { get; }
    public string Name { get; }

    public SupplierAddedEvent(Guid supplierId, string code, string name)
    {
        SupplierId = supplierId;
        Code = code;
        Name = name;
    }
}

public class SupplierRatingUpdatedEvent : BaseDomainEvent
{
    public Guid SupplierId { get; }
    public decimal OldRating { get; }
    public decimal NewRating { get; }

    public SupplierRatingUpdatedEvent(Guid supplierId, decimal oldRating, decimal newRating)
    {
        SupplierId = supplierId;
        OldRating = oldRating;
        NewRating = newRating;
    }
}