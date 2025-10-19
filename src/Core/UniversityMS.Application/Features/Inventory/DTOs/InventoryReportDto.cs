namespace UniversityMS.Application.Features.Inventory.DTOs;

public class InventoryReportDto
{
    public Guid WarehouseId { get; set; }
    public int TotalItems { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
    public DateTime GeneratedDate { get; set; }
    public List<CategoryInventoryDto> ItemsByCategory { get; set; } = new();
}