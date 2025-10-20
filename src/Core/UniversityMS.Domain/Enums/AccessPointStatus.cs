namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Noktası Durumu - Harmonized
/// </summary>
public enum AccessPointStatus
{
    Inactive = 0,            // Pasif
    Active = 1,              // Aktif
    Maintenance = 2,         // Bakımda
    Faulty = 3,              // Arızalı
    Locked = 4,              // Kilitli
    EmergencyOpen = 5        // Acil durum açık
}