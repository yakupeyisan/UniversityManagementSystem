using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Attendances.DTOs;

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid CourseRegistrationId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public int WeekNumber { get; set; }
    public bool IsPresent { get; set; }
    public AttendanceMethod Method { get; set; }
    public string? Notes { get; set; }
}