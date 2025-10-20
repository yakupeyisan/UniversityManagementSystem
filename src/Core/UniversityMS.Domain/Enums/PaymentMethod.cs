namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ödeme Yöntemi - NEW
/// </summary>
public enum PaymentMethod
{
    CreditCard = 0,          // Kredi kartı
    DebitCard = 1,           // Banka kartı
    BankTransfer = 2,        // Banka transferi
    Cash = 3,                // Nakit
    Check = 4,               // Çek
    MobilePayment = 5,       // Mobil ödeme
    Cryptocurrency = 6       // Kripto para
}