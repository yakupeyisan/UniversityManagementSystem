namespace UniversityMS.Application.Features.Inventory.DTOs;

public class CategoryInventoryDto
{
    public string Category { get; set; } = string.Empty;
    public int ItemCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
}