using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Grades.DTOs;

public record GradeDto(
    Guid CourseRegistrationId,
    Guid StudentId,
    Guid CourseId,
    GradeType GradeType,
    double NumericScore,
    double Weight,
    Guid? InstructorId = null
);