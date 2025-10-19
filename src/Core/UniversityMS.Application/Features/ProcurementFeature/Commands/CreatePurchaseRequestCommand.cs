using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;

namespace UniversityMS.Application.Features.ProcurementFeature.Commands;

public record CreatePurchaseRequestCommand(
    Guid DepartmentId,
    string Title,
    string Description,
    List<PurchaseItemDto> Items,
    string Urgency,
    DateTime? RequiredDate,
    string Priority
) : IRequest<Result<PurchaseRequestDto>>;