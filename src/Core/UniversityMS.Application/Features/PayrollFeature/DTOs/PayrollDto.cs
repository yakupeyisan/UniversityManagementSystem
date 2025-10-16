namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Output DTO - Listeleme ve detay gösterim için
/// </summary>
public class PayrollDto
{
    public Guid Id { get; set; }
    public string PayrollNumber { get; set; } = null!;
    public Guid EmployeeId { get; set; }
    public string EmployeeFullName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;

    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = null!;
    public string Period { get; set; } = null!;

    // Maaş Bilgileri
    public decimal BaseSalary { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }

    // Kalemi Detaylar
    public int WorkingDays { get; set; }
    public int ActualWorkDays { get; set; }
    public decimal OvertimeHours { get; set; }
    public int LeaveDays { get; set; }
    public int AbsentDays { get; set; }

    // Durum
    public string Status { get; set; } = null!;
    public DateTime? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }

    // Onay Bilgileri
    public DateTime? ApprovedDate { get; set; }
    public string? ApproverName { get; set; }

    // Tarihler
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public string? Notes { get; set; }
}