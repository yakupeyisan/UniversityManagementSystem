namespace UniversityMS.Application.Features.StaffFeature.DTOs;

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal WorkingHours { get; set; }
}