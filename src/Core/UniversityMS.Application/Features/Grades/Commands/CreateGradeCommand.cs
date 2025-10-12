using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Grades.Commands;

public record CreateGradeCommand(
    Guid CourseRegistrationId,
    Guid StudentId,
    Guid CourseId,
    GradeType GradeType,
    double NumericScore,
    double Weight,
    Guid? InstructorId = null
) : IRequest<Result<Guid>>;