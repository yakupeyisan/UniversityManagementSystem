namespace UniversityMS.Application.Features.Attendances.DTOs;

public class StudentAttendanceDto
{
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }
    public List<AttendanceDto> Attendances { get; set; } = new();
}