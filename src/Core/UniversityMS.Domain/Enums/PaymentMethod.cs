namespace UniversityMS.Domain.Enums;

public enum PaymentMethod
{
    Cash = 1,             // Nakit
    BankTransfer = 2,     // Havale/EFT
    Check = 3,            // Çek
    DirectDeposit = 4     // Otomatik ödeme
}