namespace UniversityMS.Application.Features.Finance.DTOs;

/// <summary>
/// Harcama Input DTO
/// </summary>
public class CreateExpenseDto
{
    public Guid BudgetId { get; set; }
    public Guid BudgetItemId { get; set; }
    public string Description { get; set; } = null!;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = null!;
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}