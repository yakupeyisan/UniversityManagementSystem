using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Procurement.Commands;

public record CreatePurchaseOrderCommand(
    Guid PurchaseRequestId,
    Guid SupplierId,
    decimal TotalAmount,
    DateTime DeliveryDate,
    string PaymentTerms
) : IRequest<Result<PurchaseOrderDto>>;