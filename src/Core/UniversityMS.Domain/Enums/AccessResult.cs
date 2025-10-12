namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Sonucu
/// </summary>
public enum AccessResult
{
    Granted = 1,              // İzin verildi
    Denied = 2,               // Reddedildi
    Error = 3,                // Hata
    ForcedOpen = 4            // Zorla açıldı
}