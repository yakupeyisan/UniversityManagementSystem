namespace UniversityMS.Domain.Enums;

/// <summary>
/// Öğün Rezervasyonu Durumu - NEW
/// </summary>
public enum MealBookingStatus
{
    Booked = 0,              // Rezerve
    Completed = 1,           // Tamamlandı
    Cancelled = 2,           // İptal
    NoShow = 3,              // Gelmedi
    InProgress = 4,          // Devam ediyor
    Ready = 5,               // Hazır
    Served = 6               // Servis yapıldı
}