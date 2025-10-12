using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Courses.DTOs;

namespace UniversityMS.Application.Features.Courses.Queries;

public record GetCourseByIdQuery(Guid Id) : IRequest<Result<CourseDto>>;