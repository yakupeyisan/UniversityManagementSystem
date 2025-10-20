using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Performans değerlendirmelerini listele
/// </summary>
public record GetPerformanceReviewListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? EmployeeId = null,
    string? Status = null,
    int? Year = null
) : IRequest<Result<PaginatedList<PerformanceReviewDto>>>;