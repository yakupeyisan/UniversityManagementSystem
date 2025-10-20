namespace UniversityMS.Domain.Enums;

/// <summary>
/// İzin Durumu - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum LeaveStatus
{
    Pending = 0,             // Beklemede
    Approved = 1,            // Onaylandı
    Rejected = 2,            // Reddedildi
    Cancelled = 3,           // İptal
    Completed = 4            // Tamamlandı
}