using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Domain.Specifications;
public class StockLevelSpecification : BaseSpecification<StockItem>
{
    public StockLevelSpecification(Guid warehouseId, StockCategory? category = null)
        : base(s =>
            s.WarehouseId == warehouseId &&
            (!category.HasValue || s.Category == category.Value))
    {
        AddOrderBy(s => s.ItemCode);
    }
    public StockLevelSpecification(Guid warehouseId, string? category)
        : base(s =>
            s.WarehouseId == warehouseId &&
            (string.IsNullOrEmpty(category) || s.Category.ToString() == category))
    {
        AddOrderBy(s => s.ItemCode);
    }
}