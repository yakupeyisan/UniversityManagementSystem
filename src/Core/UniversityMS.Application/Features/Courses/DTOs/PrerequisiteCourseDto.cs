namespace UniversityMS.Application.Features.Courses.DTOs;

public class PrerequisiteCourseDto
{
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
}