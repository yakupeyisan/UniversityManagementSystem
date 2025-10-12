namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Kayıt Tipi
/// </summary>
public enum AccessLogType
{
    Entry = 1,                // Giriş
    Exit = 2,                 // Çıkış
    Denied = 3,               // Reddedildi
    ForcedEntry = 4,          // Zorla giriş
    DoorHeldOpen = 5,         // Kapı açık tutuldu
    Tailgating = 6,           // Yetkisiz takip
    Emergency = 7,            // Acil durum
    Manual = 8                // Manuel açma
}