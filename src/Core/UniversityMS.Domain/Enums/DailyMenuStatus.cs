namespace UniversityMS.Domain.Enums;

/// <summary>
/// Günlük Menü Durumu - NEW
/// </summary>
public enum DailyMenuStatus
{
    Draft = 0,               // Taslak
    Published = 1,           // Yayınlandı
    Archived = 2,            // Arşivlendi
    Cancelled = 3,           // İptal
    Preparation = 4          // Hazırlık
}