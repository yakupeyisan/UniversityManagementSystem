namespace UniversityMS.Domain.Enums;

public enum DeductionType
{
    Tax = 1,              // Gelir Vergisi
    SocialSecurity = 2,   // SGK
    StampDuty = 3,        // Damga Vergisi
    UnemploymentInsurance = 4,  // İşsizlik Sigortası
    UnionDues = 5,        // Sendika Aidatı
    Loan = 6,             // Avans/Kredi kesintisi
    Garnishment = 7,      // Haciz/İcra
    Insurance = 8,        // Sigorta
    Pension = 9,          // Emekli sandığı
    Other = 99            // Diğer
}