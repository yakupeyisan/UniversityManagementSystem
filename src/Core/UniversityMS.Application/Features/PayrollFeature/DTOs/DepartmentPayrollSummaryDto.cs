namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Departmana Göre Bordro Özeti
/// </summary>
public class DepartmentPayrollSummaryDto
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public int EmployeeCount { get; set; }
    public decimal TotalNetSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public int ProcessedCount { get; set; }
    public int PendingCount { get; set; }
}