namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kamera Durumu
/// </summary>
public enum CameraStatus
{
    Online = 1,               // Çevrimiçi
    Offline = 2,              // Çevrimdışı
    Recording = 3,            // Kayıt yapıyor
    Maintenance = 4,          // Bakımda
    Faulty = 5                // Arızalı
}