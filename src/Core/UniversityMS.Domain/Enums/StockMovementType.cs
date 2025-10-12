namespace UniversityMS.Domain.Enums;

public enum StockMovementType
{
    In = 1,               // Giriş
    Out = 2,              // Çıkış
    Transfer = 3,         // Transfer
    Adjustment = 4,       // Düzeltme
    Return = 5,           // İade
    Damaged = 6,          // Hasarlı
    Expired = 7,          // Vadesi geçmiş
    Lost = 8              // Kayıp
}