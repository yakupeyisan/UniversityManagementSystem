namespace UniversityMS.Application.Features.Students.DTOs;

public class UpcomingExamDto
{
    public string CourseName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string Classroom { get; set; } = string.Empty;
}