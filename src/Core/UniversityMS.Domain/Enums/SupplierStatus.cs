namespace UniversityMS.Domain.Enums;

/// <summary>
/// Tedarikçi Durumu - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum SupplierStatus
{
    Active = 0,              // Aktif
    Inactive = 1,            // Pasif
    Suspended = 2,           // Askıya alındı
    Blacklisted = 3,         // Kara listeye alındı
    UnderReview = 4          // İnceleme altında
}