namespace UniversityMS.Domain.Enums;

/// <summary>
/// Payslip Durumu
/// </summary>
public enum PayslipStatus
{
    /// <summary>
    /// Taslak - Henüz oluşturulmadı
    /// </summary>
    Draft = 0,

    /// <summary>
    /// Oluşturuldu - PDF hazır
    /// </summary>
    Generated = 1,

    /// <summary>
    /// Yazdırıldı
    /// </summary>
    Printed = 2,

    /// <summary>
    /// Email gönderildi
    /// </summary>
    EmailSent = 3,

    /// <summary>
    /// İndirildi
    /// </summary>
    Downloaded = 4,

    /// <summary>
    /// E-imzalı
    /// </summary>
    Signed = 5,

    /// <summary>
    /// Onaylandı
    /// </summary>
    Approved = 6,

    /// <summary>
    /// Arşivlendi
    /// </summary>
    Archived = 7
}