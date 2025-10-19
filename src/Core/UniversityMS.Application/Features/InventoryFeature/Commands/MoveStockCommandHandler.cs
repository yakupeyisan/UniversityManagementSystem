using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.InventoryFeature.DTOs;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.InventoryFeature.Commands;

public class MoveStockCommandHandler : IRequestHandler<MoveStockCommand, Result<StockMovementDto>>
{
    private readonly IRepository<StockMovement> _movementRepository;
    private readonly IRepository<StockItem> _stockItemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MoveStockCommandHandler(
        IRepository<StockMovement> movementRepository,
        IRepository<StockItem> stockItemRepository,
        IUnitOfWork unitOfWork)
    {
        _movementRepository = movementRepository;
        _stockItemRepository = stockItemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StockMovementDto>> Handle(
        MoveStockCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var stockItem = await _stockItemRepository.GetByIdAsync(request.StockItemId, cancellationToken);
            if (stockItem == null)
                return Result<StockMovementDto>.Failure("Stok kalemi bulunamadı");

            if (stockItem.Quantity < request.Quantity)
                return Result<StockMovementDto>.Failure("Yeterli stok yok");

            // ✅ FIX: StockMovement.Create() parametrelerini düzelt
            // Doğru sıra: stockItemId, type (enum), quantity, movementDate, referenceNumber, relatedEntityId
            var movement = StockMovement.Create(
                stockItemId: request.StockItemId,                              // Guid
                type: StockMovementType.Out,                                  // ✅ StockMovementType enum
                quantity: request.Quantity,                                   // ✅ decimal
                movementDate: DateTime.UtcNow,                                // ✅ DateTime
                referenceNumber: request.Reason,                              // string?
                relatedEntityId: null                                         // Guid?
            );

            // ✅ FIX: DecreaseQuantity() metodunu ekle StockItem'e
            stockItem.DecreaseQuantity(request.Quantity);

            await _movementRepository.AddAsync(movement, cancellationToken);
            await _stockItemRepository.UpdateAsync(stockItem, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<StockMovementDto>.Success(new StockMovementDto
            {
                Id = movement.Id,
                StockItemId = movement.StockItemId,
                Quantity = movement.Quantity,
                MovementDate = movement.MovementDate,
                MovementType = movement.Type.ToString(),
                ReferenceNumber = movement.ReferenceNumber
            }, "Stok hareketi başarıyla kaydedildi.");
        }
        catch (Exception ex)
        {
            return Result<StockMovementDto>.Failure($"Stok hareketi hatası: {ex.Message}");
        }
    }
}