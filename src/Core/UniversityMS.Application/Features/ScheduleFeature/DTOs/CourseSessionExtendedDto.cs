using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ScheduleFeature.DTOs;

public class CourseSessionExtendedDto
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public Guid ClassroomId { get; set; }
    public string ClassroomCode { get; set; } = string.Empty;
    public string ClassroomName { get; set; } = string.Empty;
    public DayOfWeekEnum DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    // CourseSession'da string formatında "09:00" benzeri
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public string SessionType { get; set; } = string.Empty;
    public string? Notes { get; set; }
}