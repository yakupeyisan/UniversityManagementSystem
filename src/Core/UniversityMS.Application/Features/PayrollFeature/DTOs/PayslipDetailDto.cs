namespace UniversityMS.Application.Features.PayrollFeature.DTOs;

/// <summary>
/// Payslip Detay DTO
/// PDF/Email içeriği için detaylı maaş fişi bilgileri
/// </summary>
public class PayslipDetailDto
{
    // ========== TEMEL BİLGİLER ==========
    /// <summary>Payslip ID'si</summary>
    public Guid PayslipId { get; set; }

    /// <summary>Bordro Numarası</summary>
    public string PayrollNumber { get; set; } = null!;

    /// <summary>Payslip Dönemi (AA/YYYY)</summary>
    public string Period { get; set; } = null!;

    // ========== ÇALIŞAN BİLGİLERİ ==========
    /// <summary>Çalışan Adı Soyadı</summary>
    public string EmployeeFullName { get; set; } = null!;

    /// <summary>Çalışan Numarası</summary>
    public string EmployeeNumber { get; set; } = null!;

    /// <summary>Pozisyon/Ünvan</summary>
    public string Designation { get; set; } = null!;

    /// <summary>Departman Adı</summary>
    public string Department { get; set; } = null!;

    // ========== DÖNEM BİLGİLERİ ==========
    /// <summary>Maaş Fişi Oluşturma Tarihi</summary>
    public DateTime GeneratedDate { get; set; }

    /// <summary>Ödeme Tarihi</summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>Ödeme Yöntemi (BankTransfer, Cash, Check)</summary>
    public string? PaymentMethod { get; set; }

    // ========== MAAŞ BİLGİLERİ (ÖZETİ) ==========
    /// <summary>Temel Maaş</summary>
    public decimal BaseSalary { get; set; }

    /// <summary>Brüt Maaş (Tüm Kazançlar)</summary>
    public decimal GrossSalary { get; set; }

    /// <summary>Toplam Kesintiler</summary>
    public decimal TotalDeductions { get; set; }

    /// <summary>Net Maaş (Ödenen Tutar)</summary>
    public decimal NetSalary { get; set; }

    // ========== KAZANÇLAR (EARNINGS) ==========
    /// <summary>Bordro Kalemlerinden Kazançlar Listesi</summary>
    public List<PayslipLineItemDto> Earnings { get; set; } = new();

    /// <summary>Toplam Ek Kazançlar (İkramiye, Prim vb.)</summary>
    public decimal TotalAdditionalEarnings { get; set; }

    // ========== KESİNTİLER (DEDUCTIONS) ==========
    /// <summary>Bordro Kalemlerinden Kesintiler Listesi</summary>
    public List<PayslipLineItemDto> Deductions { get; set; } = new();

    // ========== TÜRKİYE VERGİ BİLGİLERİ ==========
    /// <summary>Gelir Vergisi (Stopaj)</summary>
    public decimal IncomeTax { get; set; }

    /// <summary>Damga Vergisi</summary>
    public decimal StampDuty { get; set; }

    /// <summary>Geçici Vergi Stopajı</summary>
    public decimal TemporaryTaxDeduction { get; set; }

    // ========== SGK BİLGİLERİ (TÜRKİYE) ==========
    /// <summary>SGK Çalışan Prim Payı (%14)</summary>
    public decimal SGKEmployeeContribution { get; set; }

    /// <summary>SGK İşveren Prim Payı (%22.2) - Bilgi Amaçlı</summary>
    public decimal SGKEmployerContribution { get; set; }

    /// <summary>İşsizlik Sigortası Çalışan Payı</summary>
    public decimal UnemploymentInsuranceEmployee { get; set; }

    /// <summary>İşsizlik Sigortası İşveren Payı - Bilgi Amaçlı</summary>
    public decimal UnemploymentInsuranceEmployer { get; set; }

    /// <summary>Sendika Aidatı (Varsa)</summary>
    public decimal? UnionFee { get; set; }

    // ========== ÇALIŞMA GÜN/SAATLERİ BİLGİLERİ ==========
    /// <summary>Takvim Çalışma Günü</summary>
    public int WorkingDays { get; set; }

    /// <summary>Gerçek Çalışma Günü</summary>
    public int ActualWorkDays { get; set; }

    /// <summary>Devamsızlık Günü</summary>
    public int AbsentDays { get; set; }

    /// <summary>İzin Günü</summary>
    public int LeaveDays { get; set; }

    /// <summary>Fazla Mesai Saati</summary>
    public decimal OvertimeHours { get; set; }

    /// <summary>Fazla Mesai Ücreti (Toplam)</summary>
    public decimal OvertimeAmount { get; set; }

    // ========== ÖDEME BİLGİLERİ ==========
    /// <summary>Banka Adı</summary>
    public string? BankName { get; set; }

    /// <summary>IBAN</summary>
    public string? IBAN { get; set; }

    /// <summary>Banka Hesap Numarası</summary>
    public string? BankAccount { get; set; }

    // ========== IMZA VE ONAY BİLGİLERİ ==========
    /// <summary>E-İmza Veri</summary>
    public string? DigitalSignature { get; set; }

    /// <summary>Onaylanmış mı?</summary>
    public bool IsApproved { get; set; }

    /// <summary>Onay Tarihi</summary>
    public DateTime? ApprovedDate { get; set; }

    /// <summary>Onaylayan Kişi</summary>
    public string? ApprovedBy { get; set; }

    /// <summary>İnsan Kaynakları Müdürü Adı</summary>
    public string? HRManagerName { get; set; }

    // ========== NOTLAR ==========
    /// <summary>Ek Notlar/Açıklamalar</summary>
    public string? Notes { get; set; }

    /// <summary>Vergi Müfettişi Açıklaması</summary>
    public string? TaxOfficerNotes { get; set; }
}