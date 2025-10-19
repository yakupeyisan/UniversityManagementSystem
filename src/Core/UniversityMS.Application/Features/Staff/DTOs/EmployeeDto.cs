namespace UniversityMS.Application.Features.Staff.DTOs;

/// <summary>
/// Employee DTO - Tüm employee bilgilerini döndürür
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public decimal BaseSalary { get; set; }
    public int WeeklyHours { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? AcademicTitle { get; set; }
    public Guid? DepartmentId { get; set; }
    public DateTime CreatedAt { get; set; }
}