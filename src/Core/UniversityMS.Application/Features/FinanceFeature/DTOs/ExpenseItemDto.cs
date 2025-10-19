namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Gider Kalemi DTO
/// </summary>
public class ExpenseItemDto
{
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}