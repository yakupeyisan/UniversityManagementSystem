using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Inventory.DTOs;
using UniversityMS.Domain.Entities.InventoryAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.Inventory.Commands;

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
        var stockItem = await _stockItemRepository.GetByIdAsync(request.StockItemId, cancellationToken);
        if (stockItem == null)
            return Result<StockMovementDto>.Failure("Stok kalemi bulunamadı");

        if (stockItem.Quantity < request.Quantity)
            return Result<StockMovementDto>.Failure("Yeterli stok yok");

        var movement = StockMovement.Create(
            request.FromWarehouseId,
            request.ToWarehouseId,
            request.StockItemId,
            request.Quantity,
            request.Reason,
            request.ValuationMethod
        );

        stockItem.DecreaseQuantity(request.Quantity);

        await _movementRepository.AddAsync(movement, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<StockMovementDto>.Success(new StockMovementDto
        {
            Id = movement.Id,
            FromWarehouseId = movement.FromWarehouseId,
            ToWarehouseId = movement.ToWarehouseId,
            StockItemId = movement.StockItemId,
            Quantity = movement.Quantity,
            MovementDate = movement.MovementDate,
            ValuationMethod = movement.ValuationMethod
        });
    }
}