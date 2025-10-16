using AutoMapper;
using FluentValidation;
using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Entities.ProcurementAggregate;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Application.Features.Procurement.Commands;

public record CreatePurchaseRequestCommand(
    Guid DepartmentId,
    string Title,
    string Description,
    List<PurchaseItemDto> Items,
    string Urgency
) : IRequest<Result<PurchaseRequestDto>>;

public record PurchaseItemDto(
    string ItemName,
    int Quantity,
    decimal UnitPrice,
    string Unit
);

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
        var purchaseRequest = new PurchaseRequest(
            request.DepartmentId,
            request.Title,
            request.Description,
            request.Urgency
        );

        foreach (var item in request.Items)
        {
            // Value Object oluştur
            var unitPrice = Money.Create(item.UnitPrice, "TRY");

            purchaseRequest.AddItem(
                item.ItemName,
                item.Quantity,
                unitPrice,
                item.Unit
            );
        }

        await _purchaseRequestRepository.AddAsync(purchaseRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PurchaseRequestDto>(purchaseRequest);
        return Result<PurchaseRequestDto>.Success(dto);
    }
}

public class CreatePurchaseRequestCommandValidator : AbstractValidator<CreatePurchaseRequestCommand>
{
    public CreatePurchaseRequestCommandValidator()
    {
        RuleFor(x => x.DepartmentId).NotEqual(Guid.Empty);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Items).NotEmpty().WithMessage("En az bir ürün eklenmelidir");
    }
}

public record CreatePurchaseOrderCommand(
    Guid PurchaseRequestId,
    Guid SupplierId,
    decimal TotalAmount,
    DateTime DeliveryDate,
    string PaymentTerms
) : IRequest<Result<PurchaseOrderDto>>;

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
            return Result.Failure<PurchaseOrderDto>("Satın alma talebi bulunamadı");

        // Value Object oluştur
        var totalAmount = Money.Create(request.TotalAmount, "TRY");

        var purchaseOrder = new PurchaseOrder(
            request.PurchaseRequestId,
            request.SupplierId,
            totalAmount,
            request.DeliveryDate,
            request.PaymentTerms
        );

        await _purchaseOrderRepository.AddAsync(purchaseOrder, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = _mapper.Map<PurchaseOrderDto>(purchaseOrder);
        return Result<PurchaseOrderDto>.Success(dto);
    }
}