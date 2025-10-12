namespace UniversityMS.Domain.Enums;

/// <summary>
/// Bakım Tipi
/// </summary>
public enum MaintenanceType
{
    Preventive = 1,           // Önleyici bakım
    Corrective = 2,           // Düzeltici bakım
    Predictive = 3,           // Öngörücü bakım
    Emergency = 4,            // Acil bakım
    Calibration = 5,          // Kalibrasyon
    Inspection = 6,           // Muayene
    Cleaning = 7,             // Temizlik
    Upgrade = 8               // Yükseltme
}