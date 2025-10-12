namespace UniversityMS.Domain.Enums;

/// <summary>
/// Rezervasyon Durumu
/// </summary>
public enum ReservationStatus
{
    Active = 1,               // Aktif
    Ready = 2,                // Hazır (alınmayı bekliyor)
    Completed = 3,            // Tamamlandı
    Cancelled = 4,            // İptal edildi
    Expired = 5               // Süresi doldu
}