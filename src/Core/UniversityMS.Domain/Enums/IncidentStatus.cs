namespace UniversityMS.Domain.Enums;

/// <summary>
/// Olay Durumu - NEW
/// </summary>
public enum IncidentStatus
{
    Reported = 0,            // Bildirildi
    Investigating = 1,       // Araştırılıyor
    UnderReview = 2,         // İnceleme altında
    Resolved = 3,            // Çözüldü
    Closed = 4,              // Kapatıldı
    Escalated = 5,           // Yükseltime alındı
    OnHold = 6               // Askıda
}