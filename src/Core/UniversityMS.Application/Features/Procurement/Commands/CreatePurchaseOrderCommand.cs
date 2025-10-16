using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Procurement.DTOs;

namespace UniversityMS.Application.Features.Procurement.Commands;

public record CreatePurchaseOrderCommand(
    Guid PurchaseRequestId,
    Guid SupplierId,
    decimal TotalAmount,
    DateTime DeliveryDate,
    string? DeliveryAddress,
    string PaymentTerms
) : IRequest<Result<PurchaseOrderDto>>;