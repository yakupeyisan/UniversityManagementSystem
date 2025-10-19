using MediatR;
using UniversityMS.Application.Common.Models;

namespace UniversityMS.Application.Features.CourseFeature.Commands;

public record UpdateCourseCommand(
    Guid Id,
    string Name,
    int TheoreticalHours,
    int PracticalHours,
    int ECTS,
    int NationalCredit,
    int? Semester,
    string? Description
) : IRequest<Result<Guid>>;