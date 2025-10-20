using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.CourseFeature.DTOs;

namespace UniversityMS.Application.Features.CourseFeature.Queries;
public record GetCourseListQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Filter = null 
) : IRequest<Result<PaginatedList<CourseDto>>>;