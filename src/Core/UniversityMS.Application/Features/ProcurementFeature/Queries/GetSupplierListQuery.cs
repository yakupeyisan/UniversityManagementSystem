using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ProcurementFeature.DTOs;

namespace UniversityMS.Application.Features.ProcurementFeature.Queries;

public record GetSupplierListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null,
    string? SearchTerm = null
) : IRequest<Result<PaginatedList<SupplierDto>>>;