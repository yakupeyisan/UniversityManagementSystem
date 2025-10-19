using UniversityMS.Application.Features.ScheduleFeature.DTOs;

namespace UniversityMS.Application.Features.ClassroomFeature.DTOs;

public class ClassroomScheduleDto
{
    public Guid ClassroomId { get; set; }
    public string ClassroomCode { get; set; } = string.Empty;
    public string ClassroomName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string Type { get; set; } = string.Empty;
    public List<CourseSessionExtendedDto> Sessions { get; set; } = new();
}