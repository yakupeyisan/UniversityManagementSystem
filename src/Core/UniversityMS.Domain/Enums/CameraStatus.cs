namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kamera Durumu
/// </summary>
public enum CameraStatus
{
    Offline = 0,             // Çevrimdışı
    Online = 1,              // Çevrimiçi
    Recording = 2,           // Kayıt yapıyor
    Standby = 3,             // Bekleme
    Maintenance = 4,         // Bakımda
    Faulty = 5               // Arızalı
}