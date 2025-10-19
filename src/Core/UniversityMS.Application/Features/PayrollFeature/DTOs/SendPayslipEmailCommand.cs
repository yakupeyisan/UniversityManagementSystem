using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Payslip email gönder komutu
/// </summary>
public record SendPayslipEmailCommand(
    Guid PayslipId
) : IRequest<Result<bool>>;