namespace UniversityMS.Application.Features.HRFeature.DTOs;

/// <summary>
/// Sözleşme DTO'su
/// </summary>
public class ContractDto
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string EmployeeNumber { get; set; } = null!;
    public string EmployeeName { get; set; } = null!;
    public string ContractNumber { get; set; } = null!;
    public string ContractType { get; set; } = null!;
    public string Status { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Salary { get; set; }
    public DateTime CreatedAt { get; set; }
}
