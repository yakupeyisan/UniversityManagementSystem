using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Schedules.DTOs;

public class WeeklyScheduleDto
{
    public Guid ScheduleId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public Dictionary<DayOfWeekEnum, List<CourseSessionDto>> Sessions { get; set; } = new();
}