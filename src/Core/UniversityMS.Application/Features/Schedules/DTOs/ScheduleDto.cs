using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Schedules.DTOs;


public class ScheduleDto
{
    public Guid Id { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ScheduleStatus Status { get; set; }
    public DateTime? PublishedDate { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalSessions { get; set; }
}