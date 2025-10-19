using UniversityMS.Domain.Entities.InventoryAggregate;

namespace UniversityMS.Domain.Specifications;

public class WarehouseStockSpecification : BaseSpecification<StockItem>
{
    public WarehouseStockSpecification(Guid warehouseId)
        : base(s => s.WarehouseId == warehouseId)
    {
        AddInclude(s => s.Warehouse);
        AddOrderBy(s => s.Category);
        AddOrderBy(s => s.ItemCode);
    }
}