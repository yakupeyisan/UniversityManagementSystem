namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ekipman Tipi
/// </summary>
public enum EquipmentType
{
    Computer = 1,             // Bilgisayar
    Projector = 2,            // Projeksiyon
    Printer = 3,              // Yazıcı
    Scanner = 4,              // Tarayıcı
    Whiteboard = 5,           // Akıllı tahta
    AudioSystem = 6,          // Ses sistemi
    VideoConference = 7,      // Video konferans
    Laboratory = 8,           // Laboratuvar ekipmanı
    MedicalDevice = 9,        // Tıbbi cihaz
    NetworkDevice = 10,       // Ağ cihazı
    SecurityDevice = 11,      // Güvenlik cihazı
    HVAC = 12,                // Isıtma/Soğutma
    Elevator = 13,            // Asansör
    Generator = 14,           // Jeneratör
    Furniture = 15,           // Mobilya
    Vehicle = 16,             // Araç
    Tool = 17,                // Alet
    Other = 99                // Diğer
}