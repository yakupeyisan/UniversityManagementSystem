using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.Courses.DTOs;

namespace UniversityMS.Application.Features.Courses.Queries;

public record GetCoursePrerequisitesQuery(Guid CourseId) : IRequest<Result<List<PrerequisiteCourseDto>>>;