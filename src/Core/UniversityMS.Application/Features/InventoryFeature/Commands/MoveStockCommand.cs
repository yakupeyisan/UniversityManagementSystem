using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.InventoryFeature.DTOs;

namespace UniversityMS.Application.Features.InventoryFeature.Commands;

public class MoveStockCommand : IRequest<Result<StockMovementDto>>
{
    public Guid FromWarehouseId { get; set; }
    public Guid ToWarehouseId { get; set; }
    public Guid StockItemId { get; set; }
    public decimal Quantity { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string ValuationMethod { get; set; } = "FIFO"; // FIFO or LIFO
}