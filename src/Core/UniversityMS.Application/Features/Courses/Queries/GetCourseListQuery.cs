using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Courses.DTOs;

namespace UniversityMS.Application.Features.Courses.Queries;



public record GetCourseListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    Guid? DepartmentId = null,
    string? SearchTerm = null,
    bool? IsActive = null
) : IRequest<Result<PaginatedList<CourseDto>>>;
