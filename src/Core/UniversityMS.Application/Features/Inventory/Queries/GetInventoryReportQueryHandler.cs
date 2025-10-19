using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Inventory.DTOs;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.Specifications;

namespace UniversityMS.Application.Features.Inventory.Queries;

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
        var spec = new WarehouseStockSpecification(request.WarehouseId);
        var items = await _stockItemRepository.ListAsync(spec, cancellationToken);

        var report = new InventoryReportDto
        {
            WarehouseId = request.WarehouseId,
            TotalItems = items.Count,
            TotalQuantity = items.Sum(i => i.Quantity),
            TotalValue = items.Sum(i => i.Quantity * i.UnitPrice.Amount),
            GeneratedDate = DateTime.UtcNow,
            ItemsByCategory = items.GroupBy(i => i.Category)
                .Select(g => new CategoryInventoryDto
                {
                    Category = g.Key,
                    ItemCount = g.Count(),
                    TotalQuantity = g.Sum(i => i.Quantity),
                    TotalValue = g.Sum(i => i.Quantity * i.UnitPrice.Amount)
                }).ToList()
        };

        return Result<InventoryReportDto>.Success(report);
    }
}