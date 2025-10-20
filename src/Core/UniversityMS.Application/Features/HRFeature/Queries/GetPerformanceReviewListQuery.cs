using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Performans değerlendirme listesi sorgu record'ı
/// </summary>
public record GetPerformanceReviewListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? EmployeeId = null,
    string? Status = null,
    string? ReviewPeriod = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<Result<PaginatedList<PerformanceReviewDto>>>;