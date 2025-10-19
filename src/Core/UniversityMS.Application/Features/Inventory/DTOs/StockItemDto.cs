namespace UniversityMS.Application.Features.Inventory.DTOs;


public class StockItemDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty; // Enum name olarak
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal TotalValue => Quantity * Price;
    public int? MinimumStock { get; set; }
    public int? MaximumStock { get; set; }
}