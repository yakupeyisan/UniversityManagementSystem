using UniversityMS.Domain.Entities.InventoryAggregate;

namespace UniversityMS.Domain.Specifications;

public class StockLevelSpecification : BaseSpecification<StockItem>
{
    public StockLevelSpecification(Guid warehouseId, string? category)
        : base(s =>
            s.WarehouseId == warehouseId &&
            (string.IsNullOrEmpty(category) || s.Category == category))
    {
        AddOrderBy(s => s.ItemCode);
    }
}