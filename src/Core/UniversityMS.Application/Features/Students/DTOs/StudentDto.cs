using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.Students.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public EducationLevel EducationLevel { get; set; }
    public int CurrentSemester { get; set; }
    public StudentStatus Status { get; set; }
    public double CGPA { get; set; }
    public double SGPA { get; set; }
    public int TotalCredits { get; set; }
    public int CompletedCredits { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public decimal Balance { get; set; }
}
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

public class TodayCourseDto
{
    public string CourseName { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string Classroom { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

public class NotificationDto
{
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}

public class UpcomingExamDto
{
    public string CourseName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string Classroom { get; set; } = string.Empty;
}
