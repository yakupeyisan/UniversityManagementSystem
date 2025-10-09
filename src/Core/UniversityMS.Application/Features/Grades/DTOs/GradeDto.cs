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

public class TranscriptDto
{
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public double CGPA { get; set; }
    public int TotalCredits { get; set; }
    public int CompletedCredits { get; set; }
    public List<GradeDto> Grades { get; set; } = new();
}

public class GradeObjectionDto
{
    public Guid Id { get; set; }
    public Guid GradeId { get; set; }
    public Guid StudentId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ObjectionDate { get; set; }
    public DateTime? ReviewDate { get; set; }
    public string? ReviewNotes { get; set; }
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
