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
        var stockItems = await _stockItemRepository.FindAsync(
            s => s.WarehouseId == request.WarehouseId &&
                 (string.IsNullOrEmpty(request.Category) || s.Category.ToString() == request.Category),
            cancellationToken);

        var dtos = stockItems.Select(s => new StockItemDto
        {
            Id = s.Id,
            ItemCode = s.ItemCode,
            ItemName = s.ItemName,
            Quantity = s.Quantity,
            Unit = s.Unit,
            Price = s.Price,
            CategoryName = s.Category.ToString(),
            TotalValue = s.Quantity * s.Price
        }).ToList();

        return Result<List<StockItemDto>>.Success(dtos);
    }
}