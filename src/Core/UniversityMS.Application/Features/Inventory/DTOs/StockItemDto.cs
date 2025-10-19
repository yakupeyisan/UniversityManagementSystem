namespace UniversityMS.Application.Features.Inventory.DTOs;


public class StockItemDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }

    // ✅ FIX: Category alanı eklendi
    public string Category { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;

    // ✅ FIX: UnitPrice property eklendi
    public decimal UnitPrice { get; set; }

    // ✅ FIX: Price property (alias olarak)
    public decimal Price => UnitPrice;

    // ✅ FIX: TotalValue backing field ile (read-write)
    private decimal? _totalValue;
    public decimal TotalValue
    {
        get => _totalValue ?? (Quantity * UnitPrice);
        set => _totalValue = value;
    }

    // Stock limits
    public int? MinimumStock { get; set; }
    public int? MaximumStock { get; set; }

    // Location & Barcode
    public string? Location { get; set; }
    public string? Barcode { get; set; }

    // Dates
    public DateTime? LastStockDate { get; set; }
}