namespace UniversityMS.Application.Features.GradeFeature.DTOs;

public class GradeDetailDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public decimal? NumericGrade { get; set; }
    public string? LetterGrade { get; set; }
    // Bunu Credits yerine ECTS olarak düşün
    public int Credits { get; set; }
}