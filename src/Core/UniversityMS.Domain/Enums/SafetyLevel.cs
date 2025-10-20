namespace UniversityMS.Domain.Enums;

/// <summary>
/// Güvenlik Seviyesi - Mevcut (1-4 → Refactored 0-3)
/// </summary>
public enum SafetyLevel
{
    Level1 = 0,              // Seviye 1 (En düşük risk)
    Level2 = 1,              // Seviye 2
    Level3 = 2,              // Seviye 3
    Level4 = 3               // Seviye 4 (En yüksek risk)
}