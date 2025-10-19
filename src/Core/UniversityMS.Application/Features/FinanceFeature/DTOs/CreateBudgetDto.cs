namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Bütçe Oluşturma Input DTO
/// </summary>
public class CreateBudgetDto
{
    public Guid DepartmentId { get; set; }
    public int Year { get; set; }
    public string BudgetType { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public string? Description { get; set; }
    public List<CreateBudgetItemDto>? Items { get; set; }
}