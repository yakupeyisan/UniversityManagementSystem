namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kampüs Durumu
/// </summary>
public enum CampusStatus
{
    Active = 1,               // Aktif
    Inactive = 2,             // Pasif
    UnderConstruction = 3,    // İnşaat halinde
    UnderRenovation = 4,      // Tadilat halinde
    Closed = 5,               // Kapalı
    Planned = 6               // Planlanmış
}