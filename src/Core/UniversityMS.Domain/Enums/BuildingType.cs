namespace UniversityMS.Domain.Enums;

/// <summary>
/// Bina Tipi - Mevcut (Tutuldu)
/// </summary>
public enum BuildingType
{
    Academic = 0,            // Akademik
    Administrative = 1,      // İdari
    Laboratory = 2,          // Laboratuvar
    Library = 3,             // Kütüphane
    Dormitory = 4,           // Yurt
    SportsFacility = 5,      // Spor tesisi
    Cafeteria = 6,           // Kafeterya
    ConferenceCenter = 7,    // Konferans merkezi
    ResearchCenter = 8,      // Araştırma merkezi
    HealthCenter = 9,        // Sağlık merkezi
    StudentCenter = 10,      // Öğrenci merkezi
    Auditorium = 11,         // Amfi
    Warehouse = 12,          // Depo
    MaintenanceBuilding = 13,// Bakım
    PowerPlant = 14,         // Enerji santrali
    Other = 99               // Diğer
}