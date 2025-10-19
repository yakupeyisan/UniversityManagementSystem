namespace UniversityMS.Application.Features.ScheduleFeature.DTOs;

public class InstructorWorkloadDto
{
    public Guid InstructorId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;
    public int Semester { get; set; }
    public int TotalCourses { get; set; }
    public int TotalHoursPerWeek { get; set; }
    public int SessionsPerWeek { get; set; }
    public bool IsOverloaded { get; set; }
    public decimal WorkloadPercentage => (TotalHoursPerWeek * 100m) / 20; // 20 saat = %100
}