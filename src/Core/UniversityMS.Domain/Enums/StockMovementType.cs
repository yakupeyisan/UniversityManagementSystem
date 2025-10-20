namespace UniversityMS.Domain.Enums;

/// <summary>
/// Stok Hareketi Türü - NEW
/// </summary>
public enum StockMovementType
{
    Inbound = 0,             // Gelen
    Outbound = 1,            // Giden
    Transfer = 2,            // Transfer
    Adjustment = 3,          // Ayarlama
    Loss = 4,                // Kayıp
    Return = 5,              // İade
    Scrapping = 6            // Hurda
}