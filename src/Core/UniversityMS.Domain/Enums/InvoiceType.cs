namespace UniversityMS.Domain.Enums;

public enum InvoiceType
{
    Sales = 1,            // Satış faturası
    Purchase = 2,         // Alış faturası
    Service = 3,          // Hizmet faturası
    CreditNote = 4,       // İade faturası
    Proforma = 5,         // Proforma fatura
    Export = 6,           // İhracat faturası
    Import = 7            // İthalat faturası
}