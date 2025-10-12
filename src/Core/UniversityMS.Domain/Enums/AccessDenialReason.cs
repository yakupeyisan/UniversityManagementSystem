namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Reddetme Sebebi
/// </summary>
public enum AccessDenialReason
{
    InvalidCredential = 1,    // Geçersiz kimlik
    ExpiredAccess = 2,        // Süresi dolmuş erişim
    NoPermission = 3,         // Yetkisiz
    TimeRestriction = 4,      // Zaman kısıtlaması
    Blacklisted = 5,          // Kara listede
    DuplicateEntry = 6,       // Çift giriş
    SystemError = 7,          // Sistem hatası
    ManualLock = 8            // Manuel kilit
}