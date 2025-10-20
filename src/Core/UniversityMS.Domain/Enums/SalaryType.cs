namespace UniversityMS.Domain.Enums;

/// <summary>
/// Maaş Türü - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum SalaryType
{
    Monthly = 0,             // Aylık
    Hourly = 1,              // Saatlik
    Daily = 2,               // Günlük
    Annual = 3               // Yıllık
}