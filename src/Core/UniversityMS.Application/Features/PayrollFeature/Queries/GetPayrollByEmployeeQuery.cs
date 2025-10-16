using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Çalışana ait tüm bordroları getir (filtrelenebilir)
/// </summary>
public record GetPayrollByEmployeeQuery(
    Guid EmployeeId,
    int? Year = null,
    int? Month = null,
    string? Status = null
) : IRequest<Result<List<PayrollDto>>>;