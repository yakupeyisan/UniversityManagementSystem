namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Raporu (Payslip) DTO
/// Maaş fişi bilgilerini taşır
/// </summary>
public class PayslipDto
{
    // ========== TEMEL BİLGİLER ==========
    public Guid Id { get; set; }
    public Guid PayrollId { get; set; }
    public Guid EmployeeId { get; set; }

    public string PayrollNumber { get; set; } = null!;
    public string EmployeeFullName { get; set; } = null!;
    public string EmployeeNumber { get; set; } = null!;
    public string Designation { get; set; } = null!;
    public string Department { get; set; } = null!;

    // ========== DÖNEM BİLGİLERİ ==========
    public string Month { get; set; } = null!;
    public int Year { get; set; }

    // ========== MAAŞ BİLGİLERİ ==========
    public decimal BaseSalary { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }

    // ========== KAZANÇLAR (EARNINGS) ==========
    public List<PayslipLineItemDto> Earnings { get; set; } = new();

    // ========== KESİNTİLER (DEDUCTIONS) ==========
    public List<PayslipLineItemDto> Deductions { get; set; } = new();

    // ========== VERGİ BİLGİLERİ (TÜRKİYE) ==========
    /// <summary>
    /// Brüt Maaş
    /// </summary>
    public decimal GrossSalary { get; set; }

    /// <summary>
    /// Gelir Vergisi
    /// </summary>
    public decimal IncomeTax { get; set; }

    /// <summary>
    /// SGK Çalışan Prim Payı
    /// </summary>
    public decimal SGKEmployeeContribution { get; set; }

    /// <summary>
    /// SGK İşveren Prim Payı (Bilgi amaçlı)
    /// </summary>
    public decimal SGKEmployerContribution { get; set; }

    // ========== ÇALIŞMA GÜN BİLGİLERİ ==========
    public int WorkingDays { get; set; }
    public int ActualWorkDays { get; set; }
    public int AbsentDays { get; set; }
    public int LeaveDays { get; set; }
    public decimal OvertimeHours { get; set; }

    // ========== TARİH BİLGİLERİ ==========
    public DateTime GeneratedDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentMethod { get; set; }

    // ========== DOSYA BİLGİLERİ ==========
    public string FilePath { get; set; } = null!;
    public string Status { get; set; } = null!;
}