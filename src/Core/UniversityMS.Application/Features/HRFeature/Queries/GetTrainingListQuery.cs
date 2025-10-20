using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.HRFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.Queries;

/// <summary>
/// Eğitim programları listesi sorgu record'ı
/// </summary>
public record GetTrainingListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Status = null,
    string? TrainingType = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    Guid? EmployeeId = null
) : IRequest<Result<PaginatedList<TrainingDto>>>;