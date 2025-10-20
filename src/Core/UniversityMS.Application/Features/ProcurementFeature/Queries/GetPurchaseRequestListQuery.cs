using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;

namespace UniversityMS.Application.Features.ProcurementFeature.Queries;


public record GetPurchaseRequestListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null,
    Guid? DepartmentId = null
) : IRequest<Result<PaginatedList<PurchaseRequestDto>>>;