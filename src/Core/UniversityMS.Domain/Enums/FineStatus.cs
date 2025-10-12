namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ceza Durumu
/// </summary>
public enum FineStatus
{
    Pending = 1,              // Beklemede
    Paid = 2,                 // Ödendi
    Waived = 3,               // Affedildi
    PartiallyPaid = 4,        // Kısmen ödendi
    Cancelled = 5             // İptal edildi
}