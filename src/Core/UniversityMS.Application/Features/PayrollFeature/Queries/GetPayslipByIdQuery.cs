using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Payslip Detayını ID'ye Göre Getir
/// </summary>
public record GetPayslipByIdQuery(
    Guid PayslipId
) : IRequest<Result<PayslipDetailDto>>;