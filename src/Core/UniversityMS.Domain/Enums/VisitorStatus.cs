namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ziyaretçi Durumu
/// </summary>
public enum VisitorStatus
{
    PreRegistered = 1,        // Ön kayıtlı
    CheckedIn = 2,            // Giriş yaptı
    InBuilding = 3,           // Binada
    CheckedOut = 4,           // Çıkış yaptı
    Expired = 5,              // Süresi doldu
    Denied = 6,               // Reddedildi
    Blacklisted = 7           // Kara listede
}