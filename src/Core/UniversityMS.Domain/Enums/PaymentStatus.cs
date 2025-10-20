namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ödeme Durumu - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum PaymentStatus
{
    Pending = 0,             // Beklemede
    Completed = 1,           // Tamamlandı
    Failed = 2,              // Başarısız
    Cancelled = 3,           // İptal
    Refunded = 4             // İade
}