namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Gelir Kalemi DTO
/// </summary>
public class IncomeItemDto
{
    public string Category { get; set; } = null!;
    public decimal Amount { get; set; }
    public int Count { get; set; }
    public decimal Percentage { get; set; }
}