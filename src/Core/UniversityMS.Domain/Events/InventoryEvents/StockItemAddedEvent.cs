using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.InventoryEvents;

public class StockItemAddedEvent : BaseDomainEvent
{
    public Guid WarehouseId { get; }
    public Guid StockItemId { get; }
    public string ItemCode { get; }

    public StockItemAddedEvent(Guid warehouseId, Guid stockItemId, string itemCode)
    {
        WarehouseId = warehouseId;
        StockItemId = stockItemId;
        ItemCode = itemCode;
    }
}