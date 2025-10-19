namespace UniversityMS.Application.Features.InventoryFeature.DTOs;

public class CategoryInventoryDto
{
    public string Category { get; set; } = null!;
    public int ItemCount { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal TotalValue { get; set; }
}