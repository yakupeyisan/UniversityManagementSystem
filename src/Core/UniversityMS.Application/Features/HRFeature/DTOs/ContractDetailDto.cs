namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Sözleşme detay DTO'su
/// </summary>
public class ContractDetailDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public string ContractNumber { get; set; } = null!;
    public string ContractType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Salary { get; set; }
    public string Terms { get; set; } = null!;
    public string? Notes { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}