namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Çalışana Ait Bordro İstatistikleri
/// </summary>
public class EmployeePayrollStatisticsDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = null!;
    public string Department { get; set; } = null!;

    public decimal AverageSalary { get; set; }
    public decimal TotalSalaryYTD { get; set; } // Year-To-Date
    public decimal TotalDeductionsYTD { get; set; }
    public decimal TotalNetYTD { get; set; }

    public int MonthsProcessed { get; set; }
    public int MonthsPending { get; set; }
    public DateTime LastPaymentDate { get; set; }
}