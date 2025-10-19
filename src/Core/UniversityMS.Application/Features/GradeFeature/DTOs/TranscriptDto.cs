namespace UniversityMS.Application.Features.GradeFeature.DTOs;

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