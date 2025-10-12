namespace UniversityMS.Domain.Enums;

public enum InvoiceStatus
{
    Draft = 1,            // Taslak
    Issued = 2,           // Kesilmiş
    Sent = 3,             // Gönderilmiş
    Paid = 4,             // Ödendi
    PartiallyPaid = 5,    // Kısmi ödendi
    Overdue = 6,          // Vadesi geçmiş
    Cancelled = 7,        // İptal
    Refunded = 8          // İade edildi
}