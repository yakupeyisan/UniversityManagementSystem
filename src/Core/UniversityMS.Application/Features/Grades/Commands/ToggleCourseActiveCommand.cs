using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.Grades.Commands;

public record ToggleCourseActiveCommand(
    Guid CourseId,
    bool IsActive
) : IRequest<Result>;