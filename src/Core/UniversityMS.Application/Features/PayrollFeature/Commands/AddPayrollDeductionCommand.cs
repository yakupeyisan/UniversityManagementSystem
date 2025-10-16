using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Bordro kesintisi ekle
/// </summary>
public record AddPayrollDeductionCommand(
    Guid PayrollId,
    string Type,
    string Description,
    decimal Amount,
    decimal? Rate = null
) : IRequest<Result<PayrollDto>>;