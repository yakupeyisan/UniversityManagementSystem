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

public class GradeObjectionDto
{
    public Guid Id { get; set; }
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ObjectionDate { get; set; }
    public Guid? ReviewedBy { get; set; }
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewNotes { get; set; }
    public double? OldScore { get; set; }
    public double? NewScore { get; set; }
}