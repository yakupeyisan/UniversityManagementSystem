namespace UniversityMS.Domain.Enums;

/// <summary>
/// Bina Tipi
/// </summary>
public enum BuildingType
{
    Academic = 1,             // Akademik bina
    Administrative = 2,       // İdari bina
    Laboratory = 3,           // Laboratuvar binası
    Library = 4,              // Kütüphane
    Dormitory = 5,            // Yurt
    SportsFacility = 6,       // Spor tesisi
    Cafeteria = 7,            // Kantin/Kafeterya
    ConferenceCenter = 8,     // Konferans merkezi
    ResearchCenter = 9,       // Araştırma merkezi
    HealthCenter = 10,        // Sağlık merkezi
    StudentCenter = 11,       // Öğrenci merkezi
    Auditorium = 12,          // Amfi/Salon
    Warehouse = 13,           // Depo
    MaintenanceBuilding = 14, // Bakım binası
    PowerPlant = 15,          // Enerji santrali
    Other = 99                // Diğer
}