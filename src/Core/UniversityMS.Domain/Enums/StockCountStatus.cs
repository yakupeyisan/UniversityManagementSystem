namespace UniversityMS.Domain.Enums;

/// <summary>
/// Stok Sayımı Durumu - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum StockCountStatus
{
    Planned = 0,             // Planlandı
    InProgress = 1,          // Devam ediyor
    Completed = 2,           // Tamamlandı
    Approved = 3,            // Onaylandı
    Cancelled = 4            // İptal
}