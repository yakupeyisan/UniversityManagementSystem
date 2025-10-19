using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Çalışan payslip'lerini listele
/// </summary>
public record GetEmployeePayslipsQuery(
    Guid EmployeeId,
    int? Year = null,
    int? Month = null
) : IRequest<Result<List<PayslipDto>>>;