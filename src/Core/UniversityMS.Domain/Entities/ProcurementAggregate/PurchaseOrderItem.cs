using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.ProcurementAggregate;

/// <summary>
/// Satın Alma Siparişi Kalemi Entity
/// </summary>
public class PurchaseOrderItem : AuditableEntity
{
    public Guid PurchaseOrderId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public string? Description { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public Money TotalAmount { get; private set; } = null!;
    public decimal ReceivedQuantity { get; private set; }

    public PurchaseOrder PurchaseOrder { get; private set; } = null!;

    private PurchaseOrderItem() { }

    private PurchaseOrderItem(
        Guid purchaseOrderId,
        string itemName,
        decimal quantity,
        string unit,
        Money unitPrice,
        string? description = null)
    {
        PurchaseOrderId = purchaseOrderId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        TotalAmount = Money.Create(quantity * unitPrice.Amount, unitPrice.Currency);
        ReceivedQuantity = 0;
        Description = description;
    }

    public static PurchaseOrderItem Create(
        Guid purchaseOrderId,
        string itemName,
        decimal quantity,
        string unit,
        Money unitPrice,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Malzeme adı boş olamaz.");

        if (quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        return new PurchaseOrderItem(purchaseOrderId, itemName, quantity, unit, unitPrice, description);
    }

    public void RecordReceipt(decimal receivedQty)
    {
        if (receivedQty <= 0)
            throw new DomainException("Teslim alınan miktar pozitif olmalıdır.");

        if (ReceivedQuantity + receivedQty > Quantity)
            throw new DomainException("Teslim alınan miktar sipariş miktarını aşamaz.");

        ReceivedQuantity += receivedQty;
    }

    public bool IsFullyReceived() => ReceivedQuantity >= Quantity;
}