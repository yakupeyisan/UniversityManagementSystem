using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Ödenmemiş/İşlenmeyen bordroları getir
/// </summary>
public record GetPendingPayrollsQuery(
    int? Month = null,
    int? Year = null
) : IRequest<Result<List<PayrollDto>>>;