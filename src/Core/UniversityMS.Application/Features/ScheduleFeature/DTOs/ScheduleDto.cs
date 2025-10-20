using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.ScheduleFeature.DTOs;


public class ScheduleDto
{
    public Guid Id { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ScheduleStatus Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime? PublishedDate { get; set; }
    public Guid? PublishedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public List<CourseSessionDto> CourseSessions { get; set; } = new();
    public int SessionCount { get; set; }
    public int TotalSessions { get; set; }
}