namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Payslip Satır Öğesi DTO
/// Maaş fişindeki kazanç veya kesinti kalemlerini temsil eder
/// </summary>
public class PayslipLineItemDto
{
    /// <summary>Kalem ID'si</summary>
    public Guid? Id { get; set; }

    /// <summary>Kalem Tipi (Earning/Deduction)</summary>
    public string Type { get; set; } = null!;

    /// <summary>Kalem Kategorisi (BaseSalary, Allowance, Bonus, Tax, Insurance vb.)</summary>
    public string Category { get; set; } = null!;

    /// <summary>Kalem Açıklaması</summary>
    public string Description { get; set; } = null!;

    /// <summary>Birim Miktar (Saat, Gün vb. için)</summary>
    public decimal? Quantity { get; set; }

    /// <summary>Birim Fiyat</summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>Tutar (TRY)</summary>
    public decimal Amount { get; set; }

    /// <summary>Oran (%)</summary>
    public decimal? Rate { get; set; }

    /// <summary>Vergilendirilir mi?</summary>
    public bool IsTaxable { get; set; }

    /// <summary>Yasal Kesinti mi?</summary>
    public bool IsStatutory { get; set; }

    /// <summary>Referans/Açıklama (Yasal madde, Karar vb.)</summary>
    public string? Reference { get; set; }

    /// <summary>Notlar</summary>
    public string? Notes { get; set; }
}
