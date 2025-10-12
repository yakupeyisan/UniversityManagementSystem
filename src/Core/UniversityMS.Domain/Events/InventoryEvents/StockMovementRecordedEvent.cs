using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.InventoryEvents;

public class StockMovementRecordedEvent : BaseDomainEvent
{
    public Guid StockItemId { get; }
    public StockMovementType Type { get; }
    public decimal Quantity { get; }

    public StockMovementRecordedEvent(Guid stockItemId, StockMovementType type, decimal quantity)
    {
        StockItemId = stockItemId;
        Type = type;
        Quantity = quantity;
    }
}