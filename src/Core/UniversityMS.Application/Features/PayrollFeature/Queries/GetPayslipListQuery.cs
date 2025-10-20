using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Tüm Payslipleri Listele (Pagination)
/// </summary>
public record GetPayslipListQuery(
    int PageNumber = 1,
    int PageSize = 50,
    Guid? EmployeeId = null,
    int? Year = null,
    int? Month = null,
    string? Status = null
) : IRequest<Result<PaginatedList<PayslipDto>>>;