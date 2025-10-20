namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Çalışan güncelleme DTO'su
/// </summary>
public class UpdateEmployeeDto
{
    public Guid EmployeeId { get; set; }
    public string? JobTitle { get; set; }
    public Guid? DepartmentId { get; set; }
    public decimal? BaseSalary { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int? WeeklyHours { get; set; }
    public string? Notes { get; set; }
}