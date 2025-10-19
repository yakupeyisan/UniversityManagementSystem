using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.InventoryFeature.DTOs;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.InventoryFeature.Queries;

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
        try
        {
            var stockItems = await _stockItemRepository.FindAsync(
                s => s.WarehouseId == request.WarehouseId &&
                     (string.IsNullOrEmpty(request.Category) || s.Category.ToString() == request.Category),
                cancellationToken);

            var dtos = stockItems.Select(s => new StockItemDto
            {
                Id = s.Id,
                WarehouseId = s.WarehouseId,
                ItemCode = s.ItemCode,
                ItemName = s.ItemName,
                Quantity = s.Quantity,
                Unit = s.Unit,
                // ✅ FIX: UnitCost.Amount kullan (UnitPrice değil)
                UnitPrice = s.UnitCost.Amount,
                CategoryName = s.Category.ToString(),
                Category = s.Category.ToString(),
                MinimumStock = (int?)s.MinimumStock,
                MaximumStock = (int?)s.MaximumStock,
                // ✅ FIX: TotalValue backing field ile (setter var)
                TotalValue = s.Quantity * s.UnitCost.Amount,
                LastStockDate = s.LastStockDate,
                Location = s.Location,
                Barcode = s.Barcode,
                Description = s.Description
            }).ToList();

            return Result<List<StockItemDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return Result<List<StockItemDto>>.Failure($"Stok seviyesi sorgusu hatası: {ex.Message}");
        }
    }
}