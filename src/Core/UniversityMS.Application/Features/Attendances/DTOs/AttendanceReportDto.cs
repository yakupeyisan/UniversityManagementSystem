namespace UniversityMS.Application.Features.Attendances.DTOs;

public class AttendanceReportDto
{
    public Guid CourseId { get; set; }
    public int TotalSessions { get; set; }
    public int TotalStudentAttendances { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double OverallAttendanceRate { get; set; }
    public List<StudentAttendanceRateDto> StudentAttendanceRates { get; set; } = new();
    public List<WeeklyAttendanceDto> WeeklyAttendance { get; set; } = new();
}