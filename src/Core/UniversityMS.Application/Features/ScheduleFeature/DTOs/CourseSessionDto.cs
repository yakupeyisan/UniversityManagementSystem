using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ScheduleFeature.DTOs;

public class CourseSessionDto
{
    public Guid Id { get; set; }
    public Guid ScheduleId { get; set; }
    public Guid CourseId { get; set; }
    public string? CourseName { get; set; }
    public string? CourseCode { get; set; }
    public Guid? InstructorId { get; set; }
    public string? InstructorName { get; set; }
    public Guid ClassroomId { get; set; }
    public string? ClassroomName { get; set; }
    public DayOfWeekEnum DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public SessionType SessionType { get; set; }
    public string? Notes { get; set; }
    public int Capacity { get; set; }
}