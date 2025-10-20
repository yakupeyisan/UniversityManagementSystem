namespace UniversityMS.Domain.Enums;

/// <summary>
/// Malzeme Durumu - Harmonized
/// </summary>
public enum MaterialStatus
{
    Available = 0,           // Mevcut
    OnLoan = 1,              // Ödünç verilmiş
    Reserved = 2,            // Rezerve edilmiş
    InProcessing = 3,        // İşlemde
    UnderMaintenance = 4,    // Bakımda
    Damaged = 5,             // Hasarlı
    Lost = 6,                // Kayıp
    Removed = 7,             // Ayıklanmış
    Reference = 8            // Kaynak (ödünç verilemez)
}