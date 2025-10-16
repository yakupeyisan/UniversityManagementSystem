namespace UniversityMS.Application.Features.Finance.DTOs;

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

/// <summary>
/// Bütçe Özeti DTO
/// </summary>
public class BudgetSummaryDto
{
    public int FiscalYear { get; set; }
    public int TotalDepartments { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal TotalAllocated { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal TotalRemaining { get; set; }
    public decimal AverageUtilizationRate { get; set; }
    public int ApprovedBudgets { get; set; }
    public int DraftBudgets { get; set; }
    public List<DepartmentBudgetSummaryDto> DepartmentSummaries { get; set; } = new();
    public DateTime GeneratedDate { get; set; }
}

/// <summary>
/// Departman Bütçe Özeti
/// </summary>
public class DepartmentBudgetSummaryDto
{
    public Guid DepartmentId { get; set; }
    public string DepartmentName { get; set; } = null!;
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public decimal UtilizationRate { get; set; }
    public int ExpenseCount { get; set; }
}

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
