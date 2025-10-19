namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Harcama Detay DTO
/// </summary>
public class ExpenseDetailDto
{
    public Guid Id { get; set; }
    public ExpenseDto Expense { get; set; } = null!;
    public decimal RemainingBudget { get; set; }
    public decimal AllowedAmount { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedDate { get; set; }
}