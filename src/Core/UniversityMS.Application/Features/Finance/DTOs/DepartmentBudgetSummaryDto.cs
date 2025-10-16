namespace UniversityMS.Application.Features.Finance.DTOs;

/// <summary>
/// Departman Bütçe Özeti
/// </summary>
public class DepartmentBudgetSummaryDto
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UtilizationRate { get; set; }
    public int ExpenseCount { get; set; }
}