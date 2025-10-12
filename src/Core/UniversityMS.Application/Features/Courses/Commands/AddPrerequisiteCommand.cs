using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Courses.Commands;

public record AddPrerequisiteCommand(
    Guid CourseId,
    Guid PrerequisiteCourseId
) : IRequest<Result>;