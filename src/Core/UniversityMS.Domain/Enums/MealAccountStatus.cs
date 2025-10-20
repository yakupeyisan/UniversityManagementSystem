namespace UniversityMS.Domain.Enums;

/// <summary>
/// Yemekhane Hesabı Durumu - NEW
/// </summary>
public enum MealAccountStatus
{
    Active = 0,              // Aktif
    Suspended = 1,           // Askıya alındı
    Closed = 2,              // Kapalı
    Blocked = 3,             // Engellendi
    Inactive = 4,            // Pasif
    Frozen = 5               // Donduruldu
}