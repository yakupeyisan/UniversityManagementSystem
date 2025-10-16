using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Commands;

/// <summary>
/// Bordro kalemi ekle
/// </summary>
public record AddPayrollItemCommand(
    Guid PayrollId,
    string Type,
    string Category,
    string Description,
    decimal Amount,
    decimal? Quantity = null,
    bool IsTaxable = true
) : IRequest<Result<PayrollDto>>;