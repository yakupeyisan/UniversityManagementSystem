namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ödünç Alma Durumu - NEW
/// </summary>
public enum LoanStatus
{
    Active = 0,              // Aktif
    Overdue = 1,             // Gecikmiş
    Returned = 2,            // İade edildi
    Lost = 3,                // Kayıp
    Cancelled = 4,           // İptal
    ClaimedLost = 5,         // Kayıp iddıası
    Renewed = 6,     // ✅ Yenilendi
    Damaged = 7      // ✅ Hasarlı
}