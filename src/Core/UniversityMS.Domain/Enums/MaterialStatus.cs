namespace UniversityMS.Domain.Enums;

/// <summary>
/// Materyal Durumu
/// </summary>
public enum MaterialStatus
{
    Available = 1,            // Mevcut
    OnLoan = 2,               // Ödünç verilmiş
    Reserved = 3,             // Rezerve edilmiş
    InProcessing = 4,         // İşlemde
    UnderMaintenance = 5,     // Bakımda
    Damaged = 6,              // Hasarlı
    Lost = 7,                 // Kayıp
    Removed = 8,              // Ayıklanmış
    Reference = 9             // Kaynak (ödünç verilemez)
}