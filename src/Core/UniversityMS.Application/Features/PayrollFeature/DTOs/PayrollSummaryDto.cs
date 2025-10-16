namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Bordro Özeti DTO
/// </summary>
public class PayrollSummaryDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string Period { get; set; } = null!;

    // İstatistikler
    public int TotalEmployees { get; set; }
    public int ProcessedPayrolls { get; set; }
    public int DraftPayrolls { get; set; }
    public int ApprovedPayrolls { get; set; }
    public int PaidPayrolls { get; set; }

    // Mali Özet
    public decimal TotalBaseSalary { get; set; }
    public decimal TotalEarnings { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TotalNetSalary { get; set; }

    // Kesinti Detayları
    public decimal TotalIncomeTax { get; set; }
    public decimal TotalSocialSecurity { get; set; }
    public decimal TotalUnemploymentInsurance { get; set; }

    // Tarihsel Veriler
    public DateTime GeneratedDate { get; set; }
}