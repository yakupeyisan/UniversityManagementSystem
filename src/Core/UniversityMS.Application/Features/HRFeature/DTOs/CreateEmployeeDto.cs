namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Çalışan oluşturma DTO'su
/// </summary>
public class CreateEmployeeDto
{
    public string EmployeeNumber { get; set; } = null!;
    public Guid PersonId { get; set; }
    public string JobTitle { get; set; } = null!;
    public DateTime HireDate { get; set; }
    public decimal BaseSalary { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int WeeklyHours { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? Notes { get; set; }
}