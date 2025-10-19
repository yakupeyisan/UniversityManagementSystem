namespace UniversityMS.Application.Features.FinanceFeature.DTOs;

/// <summary>
/// Bütçe Output DTO
/// </summary>
public class BudgetDto
{
    public Guid Id { get; set; }
    public string BudgetNumber { get; set; } = null!;
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public int FiscalYear { get; set; }
    public string BudgetType { get; set; } = null!;
    public decimal TotalAmount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UtilizationRate { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? ApprovedDate { get; set; }
    public string? ApprovedByName { get; set; }
    public string? Description { get; set; }
    public List<BudgetItemDto> Items { get; set; } = new();
    public DateTime CreatedDate { get; set; }
}