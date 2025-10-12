namespace UniversityMS.Domain.Enums;

public enum JournalEntryStatus
{
    Draft = 1,            // Taslak
    Posted = 2,           // Kaydedilmiş
    Approved = 3,         // Onaylanmış
    Reversed = 4,         // Ters kayıt
    Cancelled = 5         // İptal
}