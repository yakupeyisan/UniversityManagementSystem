namespace UniversityMS.Domain.Enums;

public enum TransactionStatus
{
    Pending = 1,          // Beklemede
    Completed = 2,        // Tamamlandı
    Failed = 3,           // Başarısız
    Cancelled = 4,        // İptal
    Reversed = 5          // İade/Ters kayıt
}