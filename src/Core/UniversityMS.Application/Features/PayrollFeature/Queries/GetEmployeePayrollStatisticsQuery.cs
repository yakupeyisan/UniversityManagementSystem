using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Çalışan bordro istatistikleri
/// </summary>
public record GetEmployeePayrollStatisticsQuery(
    Guid EmployeeId,
    int Year
) : IRequest<Result<EmployeePayrollStatisticsDto>>;