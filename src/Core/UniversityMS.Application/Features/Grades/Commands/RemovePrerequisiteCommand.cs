using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Grades.Commands;

public record RemovePrerequisiteCommand(
    Guid CourseId,
    Guid PrerequisiteCourseId
) : IRequest<Result>;