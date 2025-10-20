namespace UniversityMS.Domain.Enums;

/// <summary>
/// Lab Raporu Durumu - NEW
/// </summary>
public enum LabReportStatus
{
    Submitted = 0,           // Gönderildi
    UnderReview = 1,         // İnceleme altında
    RevisionRequested = 2,   // Revizyon talep
    Graded = 3,              // Notlandırıldı
    Returned = 4,            // İade
    Resubmitted = 5,         // Yeniden gönderildi
    Final = 6                // Son
}