namespace UniversityMS.Domain.Enums;

/// <summary>
/// Laboratuvar Durumu
/// </summary>
public enum LaboratoryStatus
{
    Operational = 0,         // Operasyonel
    UnderMaintenance = 1,    // Bakımda
    Closed = 2,              // Kapalı
    Restricted = 3,          // Sınırlı
    Emergency = 4,           // Acil durum
    Renovation = 5           // Yenileme
}