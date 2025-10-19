namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Harcama Output DTO
/// </summary>
public class ExpenseDto
{
    public Guid Id { get; set; }
    public Guid BudgetId { get; set; }
    public string BudgetNumber { get; set; } = null!;
    public Guid BudgetItemId { get; set; }
    public string BudgetItemName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedDate { get; set; }
}