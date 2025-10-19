namespace UniversityMS.Application.Features.AttendanceFeature.DTOs;

public class StudentAttendanceReportItemDto
{
    public Guid StudentId { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public decimal AttendancePercentage { get; set; }
}