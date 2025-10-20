using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Payslip Email Gönderme Command
/// Oluşturulmuş payslip'i çalışanın email adresine gönderir
/// </summary>
public record SendPayslipEmailCommand(
    /// <summary>Payslip ID'si</summary>
    Guid PayslipId,

    /// <summary>Override email adresi (opsiyonel)</summary>
    string? OverrideEmail = null,

    /// <summary>Email şablonu (opsiyonel)</summary>
    string? EmailTemplate = null
) : IRequest<Result<bool>>;