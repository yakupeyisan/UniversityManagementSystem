namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Noktası Tipi
/// </summary>
public enum AccessPointType
{
    MainEntrance = 1,         // Ana giriş
    BuildingEntrance = 2,     // Bina girişi
    DoorAccess = 3,           // Kapı erişimi
    Gate = 4,                 // Kapı/Bariyer
    Turnstile = 5,            // Turnike
    ParkingGate = 6,          // Otopark bariyeri
    ElevatorAccess = 7,       // Asansör erişimi
    LaboratoryAccess = 8,     // Laboratuvar erişimi
    SecureArea = 9,           // Güvenli alan
    EmergencyExit = 10        // Acil çıkış
}