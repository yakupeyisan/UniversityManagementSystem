namespace UniversityMS.Application.Features.CourseFeature.DTOs;

public class PrerequisiteDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid PrerequisiteCourseId { get; set; }
    public string PrerequisiteCourseName { get; set; } = string.Empty;
    public string PrerequisiteCourseCode { get; set; } = string.Empty;
}