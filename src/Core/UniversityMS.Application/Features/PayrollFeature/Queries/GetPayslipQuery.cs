using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Bordro raporunu (Payslip) getir
/// </summary>
public record GetPayslipQuery(Guid PayrollId) : IRequest<Result<PayslipDto>>;