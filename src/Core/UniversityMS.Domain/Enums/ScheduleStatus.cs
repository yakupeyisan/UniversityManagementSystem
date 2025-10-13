namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ders programı durumu
/// </summary>
public enum ScheduleStatus
{
    Draft = 1,      // Taslak
    Published = 2,  // Yayınlandı
    Active = 3,     // Aktif
    Suspended = 4,  // Askıya alındı
    Archived = 5    // Arşivlendi
}