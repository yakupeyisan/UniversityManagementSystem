using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Inventory.DTOs;

namespace UniversityMS.Application.Features.Inventory.Queries;

public class GetStockLevelQuery : IRequest<Result<List<StockItemDto>>>
{
    public Guid WarehouseId { get; set; }
    public string? Category { get; set; }
}