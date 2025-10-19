using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.AttendanceFeature.DTOs;

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public bool IsPresent { get; set; }
    public string? Notes { get; set; }
    public AttendanceMethod Method { get; set; } = AttendanceMethod.Manual;
}