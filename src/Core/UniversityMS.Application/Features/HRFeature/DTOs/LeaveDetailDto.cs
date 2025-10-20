namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// İzin detay DTO'su
/// </summary>
public class LeaveDetailDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public string LeaveType { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int DurationDays { get; set; }
    public string Status { get; set; } = null!;
    public string Reason { get; set; } = null!;
    public string? RejectionReason { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? DocumentPath { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}