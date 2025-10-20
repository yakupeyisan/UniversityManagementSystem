namespace UniversityMS.Domain.Enums;

/// <summary>
/// Badge Durumu - NEW
/// </summary>
public enum BadgeStatus
{
    Active = 0,              // Aktif
    Inactive = 1,            // Pasif
    Expired = 2,             // Süresi doldu
    Revoked = 3,             // İptal edildi
    Lost = 4,                // Kayıp
    Deactivated = 5,         // Deaktif
    Replaced = 6             // Değiştirildi
}