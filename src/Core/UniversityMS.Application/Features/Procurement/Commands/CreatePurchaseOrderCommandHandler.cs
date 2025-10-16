using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Procurement.DTOs;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Procurement.Commands;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    private readonly IRepository<PurchaseOrder> _purchaseOrderRepository;
    private readonly IRepository<PurchaseRequest> _purchaseRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePurchaseOrderCommandHandler(
        IRepository<PurchaseOrder> purchaseOrderRepository,
        IRepository<PurchaseRequest> purchaseRequestRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _purchaseRequestRepository = purchaseRequestRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(
        CreatePurchaseOrderCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseRequest = await _purchaseRequestRepository.GetByIdAsync(request.PurchaseRequestId, cancellationToken);
        if (purchaseRequest is null)
            return Result<PurchaseOrderDto>.Failure("Satın alma talebi bulunamadı");

        // Value Object oluştur
        var totalAmount = Money.Create(request.TotalAmount, "TRY");

        var purchaseOrder = PurchaseOrder.Create(
            orderNumber: $"PO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}",
            supplierId: request.SupplierId,
            orderDate: DateTime.UtcNow,
            expectedDeliveryDate: request.DeliveryDate,
            deliveryAddress: request.DeliveryAddress ?? string.Empty,
            purchaseRequestId: request.PurchaseRequestId
        );

        await _purchaseOrderRepository.AddAsync(purchaseOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PurchaseOrderDto>(purchaseOrder);
        return Result<PurchaseOrderDto>.Success(dto);
    }
}