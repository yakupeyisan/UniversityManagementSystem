namespace UniversityMS.Domain.Enums;

/// <summary>
/// Lab Oturumu Durumu - NEW
/// </summary>
public enum LabSessionStatus
{
    Scheduled = 0,           // Planlandı
    Active = 1,              // Aktif
    Completed = 2,           // Tamamlandı
    Cancelled = 3,           // İptal
    Postponed = 4,           // Ertelendi
    OnHold = 5,              // Bekleme
    Abandoned = 6            // Terk edildi
}