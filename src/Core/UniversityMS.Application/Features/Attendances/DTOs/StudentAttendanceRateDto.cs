namespace UniversityMS.Application.Features.Attendances.DTOs;

public class StudentAttendanceRateDto
{
    public Guid StudentId { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }
}