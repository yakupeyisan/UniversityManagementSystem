using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Schedules.DTOs;

public class ClassroomDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Building { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public ClassroomType Type { get; set; }
    public bool IsActive { get; set; }
    public bool HasProjector { get; set; }
    public bool HasSmartBoard { get; set; }
    public bool HasComputers { get; set; }
    public bool HasAirConditioning { get; set; }
    public int? ComputerCount { get; set; }
}