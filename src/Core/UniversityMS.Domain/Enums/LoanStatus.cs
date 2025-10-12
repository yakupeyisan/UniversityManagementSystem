namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ödünç Alma Durumu
/// </summary>
public enum LoanStatus
{
    Active = 1,               // Aktif
    Returned = 2,             // İade edildi
    Overdue = 3,              // Gecikmiş
    Renewed = 4,              // Yenilendi
    Lost = 5,                 // Kayıp
    Damaged = 6               // Hasarlı iade
}