namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Bütçe Kalemi Oluşturma DTO
/// </summary>
public class CreateBudgetItemDto
{
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public decimal PlannedAmount { get; set; }
    public string? Description { get; set; }
}