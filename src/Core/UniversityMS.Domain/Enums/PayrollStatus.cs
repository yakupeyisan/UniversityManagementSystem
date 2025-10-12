namespace UniversityMS.Domain.Enums;

public enum PayrollStatus
{
    Draft = 1,            // Taslak
    Calculated = 2,       // Hesaplanmış
    Approved = 3,         // Onaylanmış
    Rejected = 4,         // Reddedilmiş
    Paid = 5,             // Ödenmiş
    Cancelled = 6         // İptal
}