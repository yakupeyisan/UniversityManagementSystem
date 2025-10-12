namespace UniversityMS.Domain.Enums;

/// <summary>
/// Rezervasyon Durumu
/// </summary>
public enum RoomReservationStatus
{
    Pending = 1,              // Beklemede
    Confirmed = 2,            // Onaylandı
    InUse = 3,                // Kullanımda
    Completed = 4,            // Tamamlandı
    Cancelled = 5,            // İptal edildi
    NoShow = 6                // Gelilmedi
}