using AutoMapper;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public class CreatePurchaseRequestCommandHandler : IRequestHandler<CreatePurchaseRequestCommand, Result<PurchaseRequestDto>>
{
    private readonly IRepository<PurchaseRequest> _purchaseRequestRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreatePurchaseRequestCommandHandler(
        IRepository<PurchaseRequest> purchaseRequestRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _purchaseRequestRepository = purchaseRequestRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<PurchaseRequestDto>> Handle(
        CreatePurchaseRequestCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseRequest = PurchaseRequest.Create(
            requestNumber: $"PR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}",
            requestorId: Guid.NewGuid(),  // veya İZLEME: Authentication'dan al
            departmentId: request.DepartmentId,
            requestDate: DateTime.UtcNow,
            requiredDate: request.RequiredDate ?? DateTime.UtcNow.AddDays(7),
            priority: Enum.Parse<PurchasePriority>(request.Priority),
            justification: request.Description ?? "Purchase request"
        );

        foreach (var item in request.Items)
        {
            // Value Object oluştur
            var unitPrice = Money.Create(item.UnitPrice, "TRY");

            // ✅ PurchaseRequestItem.Create() factory method'u kullan
            var requestItem = PurchaseRequestItem.Create(
                purchaseRequestId: purchaseRequest.Id,
                itemName: item.ItemName,
                quantity: item.Quantity,
                unit: item.Unit,
                estimatedUnitPrice: unitPrice,
                description: item.Description,
                specifications: item.Specifications
            );

            // ✅ PurchaseRequestItem object'i geç
            purchaseRequest.AddItem(requestItem);
        }

        await _purchaseRequestRepository.AddAsync(purchaseRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PurchaseRequestDto>(purchaseRequest);
        return Result<PurchaseRequestDto>.Success(dto);
    }
}