using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Attendances.DTOs;

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid CourseRegistrationId { get; set; }
    public Guid StudentId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime AttendanceDate { get; set; }
    public int WeekNumber { get; set; }
    public bool IsPresent { get; set; }
    public AttendanceMethod Method { get; set; }
    public string? Notes { get; set; }
}

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

public class StudentAttendanceRateDto
{
    public Guid StudentId { get; set; }
    public int TotalSessions { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }
}

public class WeeklyAttendanceDto
{
    public int WeekNumber { get; set; }
    public int TotalStudents { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
    public double AttendanceRate { get; set; }
}