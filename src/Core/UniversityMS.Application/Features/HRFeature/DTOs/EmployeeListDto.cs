namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Çalışan liste DTO'su
/// </summary>
public class EmployeeListDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string? Department { get; set; }
    public string Status { get; set; } = null!;

    public EmployeeListDto() { }

    public EmployeeListDto(
        Guid id,
        string employeeNumber,
        string fullName,
        string email,
        string jobTitle,
        string? department,
        string status)
    {
        Id = id;
        EmployeeNumber = employeeNumber;
        FullName = fullName;
        Email = email;
        JobTitle = jobTitle;
        Department = department;
        Status = status;
    }
}