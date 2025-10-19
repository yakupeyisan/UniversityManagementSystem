namespace UniversityMS.Application.Features.GradeFeature.DTOs;

public class GradeStatisticsDto
{
    public Guid CourseId { get; set; }
    public int TotalStudents { get; set; }
    public double AverageGrade { get; set; }
    public double HighestGrade { get; set; }
    public double LowestGrade { get; set; }
    public decimal MedianGrade { get; set; }
    public int PassCount { get; set; }
    public int FailCount { get; set; }
    public List<GradeDetailDto> Grades { get; set; } = new();
}