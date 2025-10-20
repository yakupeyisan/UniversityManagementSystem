namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Vergi Hesaplama Sonucu DTO
/// Türkiye - Gelir Vergisi, Damga Vergisi, Stopaj detayları
/// </summary>
public class TaxCalculationResultDto
{
    // ========== TEMEL GİRİŞLER ==========
    /// <summary>Brüt Maaş</summary>
    public decimal GrossSalary { get; set; }

    /// <summary>SGK Kesintileri (SGK hesaplandıktan sonra)</summary>
    public decimal SGKDeductions { get; set; }

    /// <summary>Vergilendirilecek Gelir (Brüt - SGK)</summary>
    public decimal TaxableIncome { get; set; }

    // ========== GELIR VERGİSİ HESAPLAMALARI ==========
    /// <summary>Gelir Vergisi Matrahı (Ayarlanan)</summary>
    public decimal AdjustedTaxableIncome { get; set; }

    /// <summary>Gelir Vergisi Oranı (%)</summary>
    public decimal IncomeIncomeTaxRate { get; set; }

    /// <summary>Hesaplanan Gelir Vergisi</summary>
    public decimal IncomeTax { get; set; }

    /// <summary>Gelir Vergisi Indirim/Kesinti Oranı</summary>
    public decimal? IncomeTaxDiscount { get; set; }

    /// <summary>Gelir Vergisi Sonrası Miktar</summary>
    public decimal AfterIncomeTax { get; set; }

    // ========== STOPAJ BİLGİSİ ==========
    /// <summary>Stopaj Vergisi (%)</summary>
    public decimal WithholdingTaxRate { get; set; }

    /// <summary>Stopaj Vergisi Tutarı</summary>
    public decimal WithholdingTax { get; set; }

    // ========== DAMGA VERGİSİ ==========
    /// <summary>Damga Vergisi Oranı (%)</summary>
    public decimal StampDutyRate { get; set; }

    /// <summary>Damga Vergisi Tutarı</summary>
    public decimal StampDuty { get; set; }

    // ========== GEÇİCİ VERGİ STOPAJI ==========
    /// <summary>Geçici Vergi Stopajı (Varsa)</summary>
    public decimal TemporaryTaxDeduction { get; set; }

    /// <summary>Geçici Vergi Referansı</summary>
    public string? TemporaryTaxReference { get; set; }

    // ========== TOPLAM KESİNTİLER ==========
    /// <summary>Toplam Vergi Kesintileri</summary>
    public decimal TotalTaxDeductions { get; set; }

    /// <summary>Vergi Sonrası Net Miktar</summary>
    public decimal NetAmountAfterTax { get; set; }

    // ========== HESAPLAMA BİLGİLERİ ==========
    /// <summary>Vergi Hesaplama Yılı</summary>
    public int TaxYear { get; set; }

    /// <summary>Vergi Hesaplama Ayı</summary>
    public int TaxMonth { get; set; }

    /// <summary>Kullanılan Vergi Tablosu Versiyonu</summary>
    public string TaxTableVersion { get; set; } = "2025-Turkey";

    /// <summary>Hesaplama Tarihi ve Saati</summary>
    public DateTime CalculatedDate { get; set; }

    /// <summary>Hesaplayan Sistem/Kullanıcı</summary>
    public string? CalculatedBy { get; set; }

    // ========== AÇIKLAMA ==========
    /// <summary>Hesaplama Detay Notu</summary>
    public string? CalculationNotes { get; set; }

    /// <summary>Uygulandığı Yasal Maddeler</summary>
    public string? ApplicableLaws { get; set; }
}