namespace UniversityMS.Application.Features.AttendanceFeature.DTOs;

public class StudentAttendanceRateDto
{
    public Guid StudentId { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }
}