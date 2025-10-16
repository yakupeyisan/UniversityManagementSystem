using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Belirli ay/yıl için tüm bordroları getir
/// </summary>
public record GetPayrollByMonthQuery(
    int Year,
    int Month,
    string? Status = null
) : IRequest<Result<List<PayrollDto>>>;