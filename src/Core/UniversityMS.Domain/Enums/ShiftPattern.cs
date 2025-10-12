namespace UniversityMS.Domain.Enums;

public enum ShiftPattern
{
    Morning = 1,          // 08:00-16:00
    Afternoon = 2,        // 16:00-00:00
    Night = 3,            // 00:00-08:00
    Day = 4,              // 08:00-20:00
    Flexible = 5,         // Esnek
    Custom = 99           // Özel
}