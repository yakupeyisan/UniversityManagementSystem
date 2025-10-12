namespace UniversityMS.Domain.Enums;

/// <summary>
/// Acil Durum Alarm Tipi
/// </summary>
public enum EmergencyAlertType
{
    Fire = 1,                 // Yangın
    Earthquake = 2,           // Deprem
    MedicalEmergency = 3,     // Tıbbi acil durum
    SecurityThreat = 4,       // Güvenlik tehdidi
    Evacuation = 5,           // Tahliye
    Lockdown = 6,             // Kapatma
    ChemicalSpill = 7,        // Kimyasal sızıntı
    GasLeak = 8,              // Gaz kaçağı
    PowerOutage = 9,          // Elektrik kesintisi
    IntruderAlert = 10,       // Yetkisiz giriş
    PanicButton = 11,         // Panik butonu
    Other = 99                // Diğer
}