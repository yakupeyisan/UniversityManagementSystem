namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ekipman Tipi - Harmonized (Mevcut yapıdan)
/// </summary>
public enum EquipmentType
{
    Computer = 0,            // Bilgisayar
    Projector = 1,           // Projeksiyon
    Printer = 2,             // Yazıcı
    Scanner = 3,             // Tarayıcı
    Whiteboard = 4,          // Akıllı tahta
    AudioSystem = 5,         // Ses sistemi
    VideoConference = 6,     // Video konferans
    Laboratory = 7,          // Laboratuvar ekipmanı
    MedicalDevice = 8,       // Tıbbi cihaz
    NetworkDevice = 9,       // Ağ cihazı
    SecurityDevice = 10,     // Güvenlik cihazı
    HVAC = 11,               // Isıtma/Soğutma
    Elevator = 12,           // Asansör
    Generator = 13,          // Jeneratör
    Furniture = 14,          // Mobilya
    Vehicle = 15,            // Araç
    Tool = 16,               // Alet
    Other = 99               // Diğer
}