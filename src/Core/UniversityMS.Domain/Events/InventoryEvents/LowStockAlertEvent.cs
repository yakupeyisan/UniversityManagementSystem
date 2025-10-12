using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.InventoryEvents;

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