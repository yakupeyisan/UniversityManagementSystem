using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.InventoryFeature.DTOs;

namespace UniversityMS.Application.Features.InventoryFeature.Commands;


public class CreateStockItemCommand : IRequest<Result<StockItemDto>>
{
    public Guid WarehouseId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;

    // ✅ FIX: Category enum string olarak
    public string Category { get; set; } = string.Empty;

    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    // ✅ FIX: Minimum ve maximum stock eklendi
    public decimal? MinimumStock { get; set; }
    public decimal? MaximumStock { get; set; }

    // ✅ FIX: Opsiyonel alanlar eklendi
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Barcode { get; set; }
}