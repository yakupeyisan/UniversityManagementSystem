namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// SGK Hesaplama Sonucu DTO
/// Türkiye - Sosyal Güvenlik Kurumu ve İşsizlik Sigortası hesaplamaları
/// </summary>
public class SGKCalculationResultDto
{
    // ========== TEMEL GİRİŞ ==========
    /// <summary>Brüt Maaş</summary>
    public decimal GrossSalary { get; set; }

    /// <summary>SGK Hesaplama Matrahı</summary>
    public decimal SGKCalculationBasis { get; set; }

    // ========== SGK ÇALIŞAN PAYI (%14) ==========
    /// <summary>SGK Çalışan Prim Oranı (%)</summary>
    public decimal EmployeeContributionRate { get; set; } = 14m;

    /// <summary>SGK Çalışan Prim Tutarı</summary>
    public decimal EmployeeContributionAmount { get; set; }

    // ========== SGK İŞVEREN PAYI (%22.2) ==========
    /// <summary>SGK İşveren Prim Oranı (%)</summary>
    public decimal EmployerContributionRate { get; set; } = 22.2m;

    /// <summary>SGK İşveren Prim Tutarı</summary>
    public decimal EmployerContributionAmount { get; set; }

    // ========== İŞSİZLİK SİGORTASI ÇALIŞAN PAYI (%1) ==========
    /// <summary>İşsizlik Sigortası Çalışan Oranı (%)</summary>
    public decimal UnemploymentInsuranceEmployeeRate { get; set; } = 1m;

    /// <summary>İşsizlik Sigortası Çalışan Tutarı</summary>
    public decimal UnemploymentInsuranceEmployeeAmount { get; set; }

    // ========== İŞSİZLİK SİGORTASI İŞVEREN PAYI (%2) ==========
    /// <summary>İşsizlik Sigortası İşveren Oranı (%)</summary>
    public decimal UnemploymentInsuranceEmployerRate { get; set; } = 2m;

    /// <summary>İşsizlik Sigortası İşveren Tutarı</summary>
    public decimal UnemploymentInsuranceEmployerAmount { get; set; }

    // ========== TOPLAM ÇALIŞAN KESİNTİSİ ==========
    /// <summary>Toplam SGK Çalışan Prim Payı</summary>
    public decimal TotalEmployeeContribution { get; set; }

    /// <summary>Çalışan Kesintisi Detayı</summary>
    public string? EmployeeContributionBreakdown { get; set; }

    // ========== TOPLAM İŞVEREN MALİYETİ ==========
    /// <summary>Toplam SGK İşveren Prim Payı</summary>
    public decimal TotalEmployerContribution { get; set; }

    /// <summary>İşveren Maliyeti Detayı</summary>
    public string? EmployerContributionBreakdown { get; set; }

    /// <summary>Toplam İşveren Maliyeti (Maaş + SGK + İşsizlik)</summary>
    public decimal TotalEmployerCost { get; set; }

    // ========== ÖZEL DURUMLAR ==========
    /// <summary>Sigortalı Sayılıyor mu?</summary>
    public bool IsInsured { get; set; } = true;

    /// <summary>Prim Gün Sayısı (Ay içinde)</summary>
    public int PremiumDays { get; set; } = 30;

    /// <summary>Kesintisiz mi?</summary>
    public bool IsExempt { get; set; } = false;

    /// <summary>Muafiyet Nedeni (Varsa)</summary>
    public string? ExemptionReason { get; set; }

    // ========== HESAPLAMA BİLGİLERİ ==========
    /// <summary>SGK Hesaplama Dönemi (AA/YYYY)</summary>
    public string CalculationPeriod { get; set; } = null!;

    /// <summary>Kullanılan SGK Tarifesi Versiyonu</summary>
    public string SGKTariffVersion { get; set; } = "2025-Turkey";

    /// <summary>Hesaplama Tarihi ve Saati</summary>
    public DateTime CalculatedDate { get; set; }

    /// <summary>Hesaplayan Sistem/Kullanıcı</summary>
    public string? CalculatedBy { get; set; }

    // ========== AÇIKLAMA ==========
    /// <summary>Hesaplama Detay Notu</summary>
    public string? CalculationNotes { get; set; }

    /// <summary>Uygulandığı Yasal Referanslar</summary>
    public string? LegalReferences { get; set; }
}