using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Grades.DTOs;
public record GradeDto(
    Guid CourseRegistrationId,
    Guid StudentId,
    Guid CourseId,
    Guid? InstructorId,
    GradeType GradeType,
    double NumericScore,
    string LetterGrade,
    double GradePoint
);
