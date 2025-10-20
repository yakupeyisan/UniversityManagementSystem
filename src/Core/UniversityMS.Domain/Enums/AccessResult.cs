namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Sonucu
/// </summary>
public enum AccessResult
{
    Granted = 0,             // İzin verildi
    Denied = 1,              // Reddedildi
    Unknown = 2              // Bilinmiyor
}