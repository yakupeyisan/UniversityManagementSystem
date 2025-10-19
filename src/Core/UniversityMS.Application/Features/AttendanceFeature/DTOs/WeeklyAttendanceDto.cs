namespace UniversityMS.Application.Features.AttendanceFeature.DTOs;

public class WeeklyAttendanceDto
{
    public int WeekNumber { get; set; }
    public int TotalStudents { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }
}