namespace UniversityMS.Domain.Enums;

/// <summary>
/// Vardiya Türü - NEW
/// </summary>
public enum ShiftPattern
{
    Morning = 0,             // Sabah (08:00-16:00)
    Afternoon = 1,           // Öğleden sonra (12:00-20:00)
    Evening = 2,             // Akşam (16:00-00:00)
    Night = 3,               // Gece (00:00-08:00)
    Flexible = 4,            // Esnek
    PartTime = 5,            // Yarı zamanlı
    FullTime = 6             // Tam zamanlı
}