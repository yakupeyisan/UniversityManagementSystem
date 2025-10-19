using UniversityMS.Domain.Enums;

namespace UniversityMS.Application.Features.StaffFeature.DTOs;
public class StaffDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public string JobTitle { get; set; } = string.Empty;
    public AcademicTitle? AcademicTitle { get; set; }
    public DateTime HireDate { get; set; }
    public int YearsOfService { get; set; }
    public bool IsActive { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public decimal Balance { get; set; }
    public int? WeeklyWorkload { get; set; }
    public int? AdviseeCount { get; set; }
}