using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Inventory.DTOs;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.Inventory.Queries;

public class GetStockLevelQueryHandler : IRequestHandler<GetStockLevelQuery, Result<List<StockItemDto>>>
{
    private readonly IRepository<StockItem> _stockItemRepository;

    public GetStockLevelQueryHandler(IRepository<StockItem> stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    public async Task<Result<List<StockItemDto>>> Handle(
        GetStockLevelQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new StockLevelSpecification(request.WarehouseId, request.Category);
        var stockItems = await _stockItemRepository.ListAsync(spec, cancellationToken);

        var dtos = stockItems.Select(s => new StockItemDto
        {
            Id = s.Id,
            ItemCode = s.ItemCode,
            ItemName = s.ItemName,
            Quantity = s.Quantity,
            Unit = s.Unit,
            UnitPrice = s.UnitPrice.Amount,
            TotalValue = s.Quantity * s.UnitPrice.Amount,
            Category = s.Category
        }).ToList();

        return Result<List<StockItemDto>>.Success(dtos);
    }
}