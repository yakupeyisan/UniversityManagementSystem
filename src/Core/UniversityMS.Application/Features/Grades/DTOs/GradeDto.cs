using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Grades.DTOs;

public class GradeDto
{
    public Guid Id { get; set; }
    public Guid CourseRegistrationId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public Guid? InstructorId { get; set; }
    public GradeType GradeType { get; set; }
    public double NumericScore { get; set; }
    public string LetterGrade { get; set; } = string.Empty;
    public double GradePoint { get; set; }
    public double Weight { get; set; }
    public DateTime GradeDate { get; set; }
    public string? Notes { get; set; }
}

public class GradeStatisticsDto
{
    public Guid CourseId { get; set; }
    public int TotalStudents { get; set; }
    public double AverageScore { get; set; }
    public double AverageGradePoint { get; set; }
    public double HighestScore { get; set; }
    public double LowestScore { get; set; }
    public double PassRate { get; set; }
    public List<GradeDistributionDto> GradeDistribution { get; set; } = new();
}

public class GradeDistributionDto
{
    public string LetterGrade { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}