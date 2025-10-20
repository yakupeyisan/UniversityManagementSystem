namespace UniversityMS.Domain.Enums;

/// <summary>
/// İstihdam Durumu - NEW
/// </summary>
public enum EmploymentStatus
{
    Active = 0,              // Aktif
    OnLeave = 1,             // İzinde
    OnSabbatical = 2,        // Sabatik
    OnUnpaidLeave = 3,       // Ücretsiz izin
    Probation = 4,           // Deneme süresi
    Suspended = 5,           // Askıya alındı
    Terminated = 6,          // İşten çıkarıldı
    Retired = 7              // Emekli
}