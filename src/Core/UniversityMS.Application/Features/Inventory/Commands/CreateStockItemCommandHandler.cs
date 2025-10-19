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
        var unitPrice = Money.Create(request.UnitPrice, "TRY");

        var stockItem = StockItem.Create(
            request.WarehouseId,
            request.ItemCode,
            request.ItemName,
            request.Category,
            request.Quantity,
            request.Unit,
            unitPrice
        );

        await _stockItemRepository.AddAsync(stockItem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<StockItemDto>.Success(new StockItemDto
        {
            Id = stockItem.Id,
            ItemCode = stockItem.ItemCode,
            ItemName = stockItem.ItemName,
            Quantity = stockItem.Quantity,
            Unit = stockItem.Unit,
            UnitPrice = stockItem.UnitPrice.Amount,
            TotalValue = stockItem.Quantity * stockItem.UnitPrice.Amount
        });
    }
}