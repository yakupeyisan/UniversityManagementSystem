namespace UniversityMS.Domain.Enums;

/// <summary>
/// Rezervasyon Durumu - NEW
/// </summary>
public enum ReservationStatus
{
    Queued = 0,              // Sırada
    NotificationSent = 1,    // Bildirim gönderildi
    Fulfilled = 2,           // Karşılandı
    Cancelled = 3,           // İptal
    Expired = 4,             // Süresi doldu
    PickupExpired = 5,       // Alma süresi doldu
    OnHold = 6,               // Beklemede
    Active = 7,       // ✅ Aktif (Sırada olan ve bildirim gönderilenler)
    Ready = 8,        // ✅ Hazır (Teslime hazır)
    Completed = 9     // ✅ Tamamlandı (Teslim alındı)
}