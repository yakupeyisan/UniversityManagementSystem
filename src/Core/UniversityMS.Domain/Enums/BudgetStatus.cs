namespace UniversityMS.Domain.Enums;

public enum BudgetStatus
{
    Draft = 1,            // Taslak
    UnderReview = 2,      // İncelemede
    Approved = 3,         // Onaylanmış
    Rejected = 4,         // Reddedilmiş
    Active = 5,           // Aktif
    Closed = 6,           // Kapalı
    Revised = 7           // Revize edilmiş
}