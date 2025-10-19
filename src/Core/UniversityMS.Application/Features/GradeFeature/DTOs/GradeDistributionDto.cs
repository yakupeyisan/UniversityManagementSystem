namespace UniversityMS.Application.Features.GradeFeature.DTOs;

public class GradeDistributionDto
{
    public string LetterGrade { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}