namespace UniversityMS.Application.Features.AttendanceFeature.DTOs;

public class StudentAttendanceDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public decimal AttendancePercentage => TotalSessions > 0
        ? (PresentCount * 100m) / TotalSessions
        : 0;
    public List<AttendanceRecordDto> AttendanceRecords { get; set; } = new();
}