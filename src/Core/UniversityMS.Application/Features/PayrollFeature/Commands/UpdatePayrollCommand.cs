using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Bordroyu güncelle - Sadece taslak durumundaki bordro güncellenebilir
/// </summary>
public record UpdatePayrollCommand(
    Guid PayrollId,
    decimal? BaseSalary = null,
    decimal? Bonus = null,
    int? ActualWorkDays = null,
    int? LeaveDays = null,
    int? AbsentDays = null,
    decimal? OvertimeHours = null
) : IRequest<Result<PayrollDto>>;