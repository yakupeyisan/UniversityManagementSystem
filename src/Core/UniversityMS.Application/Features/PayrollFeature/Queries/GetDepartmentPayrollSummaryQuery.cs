using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Departmana göre bordro özeti
/// </summary>
public record GetDepartmentPayrollSummaryQuery(
    Guid DepartmentId,
    int Year,
    int Month
) : IRequest<Result<DepartmentPayrollSummaryDto>>;