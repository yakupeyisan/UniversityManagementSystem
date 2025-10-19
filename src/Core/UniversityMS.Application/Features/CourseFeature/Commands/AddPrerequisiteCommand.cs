using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.CourseFeature.Commands;

public record AddPrerequisiteCommand(
    Guid CourseId,
    Guid PrerequisiteCourseId
) : IRequest<Result>;