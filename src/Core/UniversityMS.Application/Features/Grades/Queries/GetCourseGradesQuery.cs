using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Grades.DTOs;

namespace UniversityMS.Application.Features.Grades.Queries;

public record GetCourseGradesQuery(
    Guid CourseId,
    int PageNumber = 1,
    int PageSize = 50
) : IRequest<Result<PaginatedList<GradeDto>>>;