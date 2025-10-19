using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.GradeFeature.DTOs;

namespace UniversityMS.Application.Features.GradeFeature.Queries;

public record GetCourseGradesQuery(
    Guid CourseId,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedList<GradeDto>>>;