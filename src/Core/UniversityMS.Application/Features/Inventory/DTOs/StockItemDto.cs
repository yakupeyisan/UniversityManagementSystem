namespace UniversityMS.Application.Features.Inventory.DTOs;

public class StockItemDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal TotalValue { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
}