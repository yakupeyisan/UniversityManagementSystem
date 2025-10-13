namespace UniversityMS.Application.Features.Schedules.DTOs;

public class ScheduleConflictDto
{
    public bool HasConflict { get; set; }
    public string ConflictType { get; set; } = string.Empty; // "Classroom", "Instructor"
    public string ConflictMessage { get; set; } = string.Empty;
    public CourseSessionDto? ConflictingSession { get; set; }
}