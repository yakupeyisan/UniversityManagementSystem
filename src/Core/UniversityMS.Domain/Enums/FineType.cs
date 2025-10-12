namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ceza Tipi
/// </summary>
public enum FineType
{
    LateFee = 1,              // Gecikme ücreti
    DamageFee = 2,            // Hasar bedeli
    LostItemFee = 3,          // Kayıp materyal bedeli
    ReplacementFee = 4,       // Yenileme bedeli
    ProcessingFee = 5         // İşlem ücreti
}