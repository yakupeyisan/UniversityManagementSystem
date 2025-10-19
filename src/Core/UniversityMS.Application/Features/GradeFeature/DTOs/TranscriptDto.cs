namespace UniversityMS.Application.Features.GradeFeature.DTOs;

public class TranscriptDto
{
    public Guid StudentId { get; set; }
    public decimal GPA { get; set; }
    public int TotalCredits { get; set; }
    public int CompletedCredits { get; set; }
    public int TotalCourses { get; set; }
    public int PassedCourses { get; set; }
    public int FailedCourses { get; set; }
    public double AverageGrade { get; set; }
    public List<TranscriptCourseDto> Courses { get; set; } = new();
    public DateTime GeneratedDate { get; set; }
}