namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Noktası Durumu
/// </summary>
public enum AccessPointStatus
{
    Active = 1,               // Aktif
    Inactive = 2,             // Pasif
    Maintenance = 3,          // Bakımda
    Faulty = 4,               // Arızalı
    Locked = 5,               // Kilitli
    EmergencyOpen = 6         // Acil durum açık
}