namespace UniversityMS.Domain.Enums;

/// <summary>
/// Satın Alma Türü Durumu - NEW
/// </summary>
public enum PurchaseRequestStatus
{
    Draft = 0,               // Taslak
    Submitted = 1,           // Gönderildi
    Approved = 2,            // Onaylandı
    Rejected = 3,            // Reddedildi
    Processing = 4,          // İşlemde
    Cancelled = 5,           // İptal
    Completed = 6            // Tamamlandı
}