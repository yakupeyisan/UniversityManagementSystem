using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.InventoryEvents;

public class WarehouseCreatedEvent : BaseDomainEvent
{
    public Guid WarehouseId { get; }
    public string Code { get; }
    public string Name { get; }
    public Guid CampusId { get; }

    public WarehouseCreatedEvent(Guid warehouseId, string code, string name, Guid campusId)
    {
        WarehouseId = warehouseId;
        Code = code;
        Name = name;
        CampusId = campusId;
    }
}

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

public class LowStockAlertEvent : BaseDomainEvent
{
    public Guid StockItemId { get; }
    public string ItemCode { get; }
    public decimal CurrentQuantity { get; }
    public decimal MinimumStock { get; }

    public LowStockAlertEvent(Guid stockItemId, string itemCode, decimal currentQuantity, decimal minimumStock)
    {
        StockItemId = stockItemId;
        ItemCode = itemCode;
        CurrentQuantity = currentQuantity;
        MinimumStock = minimumStock;
    }
}

public class OutOfStockAlertEvent : BaseDomainEvent
{
    public Guid StockItemId { get; }
    public string ItemCode { get; }

    public OutOfStockAlertEvent(Guid stockItemId, string itemCode)
    {
        StockItemId = stockItemId;
        ItemCode = itemCode;
    }
}