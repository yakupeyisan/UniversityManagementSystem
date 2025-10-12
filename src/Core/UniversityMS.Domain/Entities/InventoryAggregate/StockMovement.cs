using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.InventoryAggregate;

/// <summary>
/// Stok Hareketi (StockMovement) Entity
/// </summary>
public class StockMovement : AuditableEntity
{
    public Guid StockItemId { get; private set; }
    public StockMovementType Type { get; private set; }
    public decimal Quantity { get; private set; }
    public DateTime MovementDate { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public Guid? RelatedEntityId { get; private set; } // PurchaseOrder, Transfer vs.
    public string? Notes { get; private set; }

    public StockItem StockItem { get; private set; } = null!;

    private StockMovement() { }

    private StockMovement(
        Guid stockItemId,
        StockMovementType type,
        decimal quantity,
        DateTime movementDate,
        string? referenceNumber = null,
        Guid? relatedEntityId = null)
    {
        StockItemId = stockItemId;
        Type = type;
        Quantity = quantity;
        MovementDate = movementDate;
        ReferenceNumber = referenceNumber;
        RelatedEntityId = relatedEntityId;
    }

    public static StockMovement Create(
        Guid stockItemId,
        StockMovementType type,
        decimal quantity,
        DateTime movementDate,
        string? referenceNumber = null,
        Guid? relatedEntityId = null)
    {
        if (quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        return new StockMovement(stockItemId, type, quantity, movementDate, referenceNumber, relatedEntityId);
    }
}