namespace UniversityMS.Domain.Enums;

/// <summary>
/// Malzeme Kopyası Durumu - NEW
/// </summary>
public enum MaterialCopyStatus
{
    Available = 0,           // Mevcut
    CheckedOut = 1,          // Ödünç verildi
    Reserved = 2,            // Rezerve
    Damaged = 3,             // Hasarlı
    Lost = 4,                // Kayıp
    InRepair = 5,            // Onarımda
    Discarded = 6,           // Hurdaya ayrıldı
    NotFound = 7             // Bulunamadı
}