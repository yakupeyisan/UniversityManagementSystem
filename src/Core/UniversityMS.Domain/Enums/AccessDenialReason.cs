namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Reddedilme Nedeni
/// </summary>
public enum AccessDenialReason
{
    InvalidCredential = 0,       // Geçersiz kimlik
    AccessLevelInsufficient = 1, // Yetersiz yetki
    TimeRestriction = 2,         // Zaman kısıtlaması
    ZoneRestriction = 3,         // Bölge kısıtlaması
    CredentialExpired = 4,       // Kimlik süresi doldu
    CredentialBlacklisted = 5,   // Kimlik kara listeye alındı
    SystemError = 6,             // Sistem hatası
    EmergencyLock = 7,           // Acil kilit
    UnknownReason = 8            // Bilinmeyen neden
}