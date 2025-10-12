namespace UniversityMS.Application.Features.Students.DTOs;

public class StudentDashboardDto
{
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public double CGPA { get; set; }
    public double SGPA { get; set; }
    public int CurrentSemester { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public List<TodayCourseDto> TodayCourses { get; set; } = new();
    public List<NotificationDto> RecentNotifications { get; set; } = new();
    public List<UpcomingExamDto> UpcomingExams { get; set; } = new();
}