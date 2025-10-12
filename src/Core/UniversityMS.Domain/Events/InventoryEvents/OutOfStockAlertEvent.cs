using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.InventoryEvents;

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