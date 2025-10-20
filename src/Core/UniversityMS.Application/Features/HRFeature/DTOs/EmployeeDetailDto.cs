namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Çalışan detay DTO'su
/// </summary>
public class EmployeeDetailDto
{
    public Guid Id { get; set; }
    public string EmployeeNumber { get; set; } = null!;
    public Guid PersonId { get; set; }
    public string PersonName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string JobTitle { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime HireDate { get; set; }
    public DateTime? TerminationDate { get; set; }
    public Guid? DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
    public decimal BaseSalary { get; set; }
    public int AnnualLeaveBalance { get; set; }
    public int TenureYears { get; set; }
    public List<ContractDto> Contracts { get; set; } = new();
    public List<LeaveRequestDto> Leaves { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}