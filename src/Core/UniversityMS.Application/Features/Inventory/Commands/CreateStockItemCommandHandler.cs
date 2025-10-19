using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Inventory.DTOs;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Inventory.Commands;

public class CreateStockItemCommandHandler : IRequestHandler<CreateStockItemCommand, Result<StockItemDto>>
{
    private readonly IRepository<StockItem> _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateStockItemCommandHandler(
        IRepository<StockItem> stockItemRepository,
        IUnitOfWork unitOfWork)
    {
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StockItemDto>> Handle(
        CreateStockItemCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            // ✅ Money object oluştur
            var unitCost = Money.Create(request.UnitPrice, "TRY");

            // ✅ FIX: StockItem.Create() parametrelerini düzelt
            // Beklenen sıra: warehouseId, itemCode, itemName, category, unit, minimumStock, maximumStock, unitCost, description
            var stockItem = StockItem.Create(
                warehouseId: request.WarehouseId,           // Guid
                itemCode: request.ItemCode,                 // string
                itemName: request.ItemName,                 // string
                category: Enum.Parse<UniversityMS.Domain.Enums.StockCategory>(request.Category), // StockCategory enum
                unit: request.Unit,                         // string
                minimumStock: request.MinimumStock ?? 10,   // decimal (default 10)
                maximumStock: request.MaximumStock ?? 100,  // decimal (default 100)
                unitCost: unitCost,                         // Money
                description: request.Description            // string? (opsiyonel)
            );

            await _stockItemRepository.AddAsync(stockItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<StockItemDto>.Success(new StockItemDto
            {
                Id = stockItem.Id,
                WarehouseId = stockItem.WarehouseId,
                ItemCode = stockItem.ItemCode,
                ItemName = stockItem.ItemName,
                Quantity = stockItem.Quantity,
                Unit = stockItem.Unit,
                // ✅ FIX: UnitPrice property ekle
                UnitPrice = stockItem.UnitCost.Amount,
                // ✅ FIX: TotalValue computed property olarak
                TotalValue = stockItem.Quantity * stockItem.UnitCost.Amount,
                CategoryName = stockItem.Category.ToString(),
                MinimumStock = (int?)stockItem.MinimumStock,
                MaximumStock = (int?)stockItem.MaximumStock,
                Description = stockItem.Description
            }, "Stok kalemi başarıyla oluşturuldu.");
        }
        catch (Exception ex)
        {
            return Result<StockItemDto>.Failure($"Stok kalemi oluşturma hatası: {ex.Message}");
        }
    }
}