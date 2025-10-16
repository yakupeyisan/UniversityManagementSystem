using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.PayrollFeature.DTOs;

namespace UniversityMS.Application.Features.PayrollFeature.Queries;

/// <summary>
/// Ay için bordro özeti getir
/// </summary>
public record GetPayrollSummaryQuery(
    int Year,
    int Month
) : IRequest<Result<PayrollSummaryDto>>;