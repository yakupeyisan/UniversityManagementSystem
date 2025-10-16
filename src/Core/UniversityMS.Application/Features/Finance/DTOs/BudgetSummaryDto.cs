namespace UniversityMS.Application.Features.Finance.DTOs;

/// <summary>
/// Bütçe Özeti DTO
/// </summary>
public class BudgetSummaryDto
{
    public int FiscalYear { get; set; }
    public int TotalDepartments { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalAllocated { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRemaining { get; set; }
    public decimal AverageUtilizationRate { get; set; }
    public int ApprovedBudgets { get; set; }
    public int DraftBudgets { get; set; }
    public List<DepartmentBudgetSummaryDto> DepartmentSummaries { get; set; } = new();
    public DateTime GeneratedDate { get; set; }
}