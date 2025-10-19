namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Bütçe Kalemi DTO
/// </summary>
public class BudgetItemDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal PlannedAmount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UtilizationPercentage { get; set; }
    public string? Description { get; set; }
}