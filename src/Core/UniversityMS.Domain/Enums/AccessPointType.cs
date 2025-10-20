namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Noktası Türü - Harmonized (Mevcut: 10 değer → Refactored)
/// </summary>
public enum AccessPointType
{
    Door = 0,                 // Kapı erişimi
    MainEntrance = 1,        // Ana giriş
    BuildingEntrance = 2,    // Bina girişi
    Gate = 3,                // Kapı/Bariyer
    Turnstile = 4,           // Turnike
    ParkingGate = 5,         // Otopark bariyeri
    ElevatorAccess = 6,      // Asansör erişimi
    LaboratoryAccess = 7,    // Laboratuvar erişimi
    SecureArea = 8,          // Güvenli alan
    EmergencyExit = 9        // Acil çıkış
}