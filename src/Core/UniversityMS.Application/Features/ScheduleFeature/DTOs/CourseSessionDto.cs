using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ScheduleFeature.DTOs;

public class CourseSessionDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public Guid ClassroomId { get; set; }
    public string ClassroomName { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int WeekNumber { get; set; }
    public int DurationInMinutes => (int)(EndTime - StartTime).TotalMinutes;
}
