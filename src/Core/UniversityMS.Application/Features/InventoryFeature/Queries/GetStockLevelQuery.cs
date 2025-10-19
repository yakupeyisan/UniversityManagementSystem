using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.InventoryFeature.DTOs;

namespace UniversityMS.Application.Features.InventoryFeature.Queries;

public class GetStockLevelQuery : IRequest<Result<List<StockItemDto>>>
{
    public Guid WarehouseId { get; set; }
    public string? Category { get; set; }
}