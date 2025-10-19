namespace UniversityMS.Application.Features.GradeFeature.DTOs;

public class TranscriptCourseDto
{
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public int Credits { get; set; }
    public decimal? NumericGrade { get; set; }
    public string? LetterGrade { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
}