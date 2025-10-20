using UniversityMS.Application.Features.StaffFeature.DTOs;

namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Çalışan DTO'su
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = null!;
    public string PersonName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Department { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public decimal BaseSalary { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}