namespace UniversityMS.Domain.Enums;

/// <summary>
/// Bina Durumu
/// </summary>
public enum BuildingStatus
{
    Active = 1,               // Aktif
    UnderConstruction = 2,    // İnşaat halinde
    UnderRenovation = 3,      // Tadilat halinde
    Maintenance = 4,          // Bakımda
    Closed = 5,               // Kapalı
    Demolished = 6            // Yıkılmış
}