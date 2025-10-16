using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Tüm bordroları sayfalama ile getir
/// </summary>
public record GetAllPayrollsQuery(
    int PageNumber = 1,
    int PageSize = 50,
    string? Status = null,
    int? Year = null,
    int? Month = null
) : IRequest<Result<PaginatedList<PayrollDto>>>;