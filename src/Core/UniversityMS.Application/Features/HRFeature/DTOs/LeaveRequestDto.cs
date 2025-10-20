namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// İzin talep DTO'su
/// </summary>
public class LeaveRequestDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string LeaveType { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public string Status { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}