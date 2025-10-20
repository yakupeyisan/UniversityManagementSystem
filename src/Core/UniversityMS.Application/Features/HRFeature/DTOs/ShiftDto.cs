namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Vardiya DTO'su
/// </summary>
public class ShiftDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string ShiftPattern { get; set; } = null!;
    public string Status { get; set; } = null!;
    public decimal? OvertimeHours { get; set; }
    public DateTime CreatedAt { get; set; }
}