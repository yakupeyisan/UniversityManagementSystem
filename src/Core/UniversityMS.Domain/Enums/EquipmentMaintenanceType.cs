namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ekipman Bakım Türü - NEW
/// </summary>
public enum EquipmentMaintenanceType
{
    Preventive = 0,          // Koruyucu bakım
    Corrective = 1,          // Düzeltici bakım
    Calibration = 2,         // Kalibrasyon
    Inspection = 3,          // İnceleme
    Repair = 4,              // Onarım
    Cleaning = 5,            // Temizlik
    Upgrade = 6              // Yükseltme
}