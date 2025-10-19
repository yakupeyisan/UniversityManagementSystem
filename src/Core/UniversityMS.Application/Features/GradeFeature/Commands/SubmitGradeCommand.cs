using MediatR;
using UniversityMS.Application.Common.Models;
using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.GradeFeature.Commands;

public record SubmitGradeCommand(
    Guid CourseRegistrationId,
    Guid StudentId,
    Guid CourseId,
    GradeType GradeType,
    double NumericScore,
    double Weight,
    Guid? InstructorId = null,
    string? Notes = null
) : IRequest<Result<Guid>>;