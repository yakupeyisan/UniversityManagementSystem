namespace UniversityMS.Application.Features.StudentFeature.DTOs;

public class TodayCourseDto
{
    public string CourseName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string Classroom { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}