using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.InventoryFeature.DTOs;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.InventoryFeature.Queries;

public class GetInventoryReportQueryHandler : IRequestHandler<GetInventoryReportQuery, Result<InventoryReportDto>>
{
    private readonly IRepository<StockItem> _stockItemRepository;

    public GetInventoryReportQueryHandler(IRepository<StockItem> stockItemRepository)
    {
        _stockItemRepository = stockItemRepository;
    }

    public async Task<Result<InventoryReportDto>> Handle(
        GetInventoryReportQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var items = await _stockItemRepository.FindAsync(
                s => s.WarehouseId == request.WarehouseId,
                cancellationToken);

            var report = new InventoryReportDto
            {
                WarehouseId = request.WarehouseId,
                TotalItems = items.Count,
                TotalQuantity = items.Sum(i => i.Quantity),
                // ✅ FIX: UnitCost (Money) → Amount (decimal)
                TotalValue = items.Sum(i => i.Quantity * i.UnitCost.Amount),
                GeneratedDate = DateTime.UtcNow,
                // ✅ FIX: Category enum → string dönüşümü
                ItemsByCategory = items
                    .GroupBy(i => i.Category)
                    .Select(g => new CategoryInventoryDto
                    {
                        Category = g.Key.ToString(),        // ✅ Enum → string
                        ItemCount = g.Count(),
                        TotalQuantity = g.Sum(i => i.Quantity),
                        TotalValue = g.Sum(i => i.Quantity * i.UnitCost.Amount)  // ✅ UnitCost kullan
                    }).ToList()
            };

            return Result<InventoryReportDto>.Success(report);
        }
        catch (Exception ex)
        {
            return Result<InventoryReportDto>.Failure($"Stok raporu hatası: {ex.Message}");
        }
    }
}
