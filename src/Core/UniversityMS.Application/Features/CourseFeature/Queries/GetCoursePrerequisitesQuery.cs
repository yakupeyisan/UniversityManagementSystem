using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.CourseFeature.DTOs;

namespace UniversityMS.Application.Features.CourseFeature.Queries;

public record GetCoursePrerequisitesQuery(Guid CourseId) : IRequest<Result<List<PrerequisiteCourseDto>>>;