namespace UniversityMS.Domain.Enums;

public enum TenderType
{
    OpenTender = 1,           // Açık ihale
    RestrictedTender = 2,     // Belli istekliler arası ihale
    Negotiation = 3,          // Pazarlık usulü
    DirectProcurement = 4,    // Doğrudan temin
    TwoStage = 5,             // İki aşamalı ihale
    Framework = 6             // Çerçeve anlaşma
}