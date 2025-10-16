using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Procurement.DTOs;

namespace UniversityMS.Application.Features.Procurement.Commands;

public record CreatePurchaseRequestCommand(
    Guid DepartmentId,
    string Title,
    string Description,
    List<PurchaseItemDto> Items,
    string Urgency
) : IRequest<Result<PurchaseRequestDto>>;