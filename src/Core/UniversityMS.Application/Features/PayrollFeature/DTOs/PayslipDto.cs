namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Raporu (Payslip) DTO
/// </summary>
public class PayslipDto
{
    public Guid PayrollId { get; set; }
    public string PayrollNumber { get; set; } = null!;
    public string EmployeeFullName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public string Department { get; set; } = null!;

    public string Month { get; set; } = null!;
    public int Year { get; set; }

    // Earnings
    public decimal BaseSalary { get; set; }
    public List<PayslipLineItemDto> Earnings { get; set; } = new();
    public decimal TotalEarnings { get; set; }

    // Deductions
    public List<PayslipLineItemDto> Deductions { get; set; } = new();
    public decimal TotalDeductions { get; set; }

    // Net Result
    public decimal NetSalary { get; set; }

    // Working Info
    public int WorkingDays { get; set; }
    public int ActualWorkDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public decimal OvertimeHours { get; set; }

    // Dates
    public DateTime GeneratedDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public string FilePath { get; set; } = null!;
    public string Status { get; set; } = null!;
}