namespace UniversityMS.Application.Features.StaffFeature.DTOs;

/// <summary>
/// Employee DTO - Tüm employee bilgilerini döndürür
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string EmploymentType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
