namespace UniversityMS.Domain.Enums;

/// <summary>
/// Güvenlik Bölgesi Durumu - NEW
/// </summary>
public enum SecurityZoneStatus
{
    Active = 0,              // Aktif
    Inactive = 1,            // Pasif
    UnderMaintenance = 2,    // Bakımda
    Restricted = 3,          // Sınırlı
    Closed = 4               // Kapalı
}