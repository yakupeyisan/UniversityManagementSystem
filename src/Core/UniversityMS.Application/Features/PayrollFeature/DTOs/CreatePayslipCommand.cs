using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Payslip oluştur komutu
/// </summary>
public record CreatePayslipCommand(
    Guid PayrollId
) : IRequest<Result<PayslipDto>>;