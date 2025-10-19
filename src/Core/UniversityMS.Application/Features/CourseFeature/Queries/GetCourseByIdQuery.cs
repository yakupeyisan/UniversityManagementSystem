using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.CourseFeature.DTOs;

namespace UniversityMS.Application.Features.CourseFeature.Queries;

public record GetCourseByIdQuery(Guid Id) : IRequest<Result<CourseDto>>;