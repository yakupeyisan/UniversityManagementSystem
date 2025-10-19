namespace UniversityMS.Application.Features.AttendanceFeature.DTOs;

public class AttendanceReportDto
{
    public Guid CourseId { get; set; }
    public int TotalStudents { get; set; }
    public int TotalSessions { get; set; }
    public decimal OverallPresentPercentage { get; set; }
    public DateTime GeneratedDate { get; set; }
    public List<StudentAttendanceReportItemDto> StudentAttendances { get; set; } = new();
}