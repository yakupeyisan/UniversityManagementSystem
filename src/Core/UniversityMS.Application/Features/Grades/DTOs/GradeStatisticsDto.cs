namespace UniversityMS.Application.Features.Grades.DTOs;

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