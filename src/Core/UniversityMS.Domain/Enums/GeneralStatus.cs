namespace UniversityMS.Domain.Enums;

/// <summary>
/// Genel Durum - NEW
/// </summary>
public enum GeneralStatus
{
    Active = 0,              // Aktif
    Inactive = 1,            // Pasif
    Pending = 2,             // Beklemede
    Archived = 3,            // Arşivlendi
    Deleted = 4              // Silindi
}