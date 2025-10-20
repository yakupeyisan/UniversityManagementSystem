using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Eğitimleri listele
/// </summary>
public record GetTrainingListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? EmployeeId = null,
    string? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<Result<PaginatedList<TrainingDto>>>;