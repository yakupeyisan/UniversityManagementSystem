using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public record ToggleCourseActiveCommand(
    Guid CourseId,
    bool IsActive
) : IRequest<Result>;