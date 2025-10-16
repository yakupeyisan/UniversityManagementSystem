namespace UniversityMS.Application.Features.Finance.DTOs;

/// <summary>
/// Mali Rapor DTO
/// </summary>
public class FinancialReportDto
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public int Year { get; set; }
    public string Period { get; set; } = null!;

    // Gelirler
    public decimal TotalIncome { get; set; }
    public List<IncomeItemDto> IncomeItems { get; set; } = new();

    // Giderler
    public decimal TotalExpenses { get; set; }
    public List<ExpenseItemDto> ExpenseItems { get; set; } = new();

    // Net
    public decimal NetBalance { get; set; }

    // Istatistikler
    public decimal IncomeExpenseRatio { get; set; }
    public DateTime GeneratedDate { get; set; }
}