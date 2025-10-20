namespace UniversityMS.Domain.Enums;

/// <summary>
/// Bakım Durumu - NEW
/// </summary>
public enum MaintenanceStatus
{
    Scheduled = 0,           // Planlandı
    InProgress = 1,          // Devam ediyor
    OnHold = 2,              // Bekleme
    Completed = 3,           // Tamamlandı
    Cancelled = 4,           // İptal
    Failed = 5               // Başarısız
}