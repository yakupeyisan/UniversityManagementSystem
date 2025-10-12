using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Grades.DTOs;
public class GradeDto
{
    public Guid Id { get; set; }
    public Guid CourseRegistrationId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public Guid? InstructorId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public GradeType GradeType { get; set; }
    public double NumericScore { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public double GradePoint { get; set; }
    public double Weight { get; set; }
    public DateTime GradeDate { get; set; }
    public string? Notes { get; set; }
}