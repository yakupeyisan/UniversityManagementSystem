using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Schedules.DTOs;

public class CourseSessionDto
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public Guid? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public Guid ClassroomId { get; set; }
    public string ClassroomCode { get; set; } = string.Empty;
    public string ClassroomName { get; set; } = string.Empty;
    public DayOfWeekEnum DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public SessionType SessionType { get; set; }
    public string? Notes { get; set; }
}