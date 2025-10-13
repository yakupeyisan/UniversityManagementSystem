namespace UniversityMS.Application.Features.Schedules.DTOs;

public class InstructorWorkloadDto
{
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public int TotalWeeklyHours { get; set; }
    public int TotalSessions { get; set; }
    public List<CourseSessionDto> Sessions { get; set; } = new();
}