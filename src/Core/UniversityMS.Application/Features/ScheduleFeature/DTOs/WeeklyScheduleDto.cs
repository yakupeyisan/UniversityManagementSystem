using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ScheduleFeature.DTOs;

public class WeeklyScheduleDto
{
    public Guid ScheduleId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    // CourseSessionExtendedDto'yu kullan
    public List<CourseSessionExtendedDto> Sessions { get; set; } = new();
}