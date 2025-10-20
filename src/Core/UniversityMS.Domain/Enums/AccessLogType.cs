namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Kayıt Türü - Harmonized
/// </summary>
public enum AccessLogType
{
    Entry = 0,               // Giriş
    Exit = 1,                // Çıkış
    Denied = 2,              // Reddedildi
    ForcedEntry = 3,         // Zorla giriş
    DoorHeldOpen = 4,        // Kapı açık tutuldu
    Tailgating = 5,          // Yetkisiz takip
    Emergency = 6,           // Acil durum
    Manual = 7               // Manuel açma
}