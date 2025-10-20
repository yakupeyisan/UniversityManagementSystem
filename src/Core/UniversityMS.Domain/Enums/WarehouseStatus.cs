namespace UniversityMS.Domain.Enums;

/// <summary>
/// Depo Durumu - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum WarehouseStatus
{
    Active = 0,              // Aktif
    Inactive = 1,            // Pasif
    Maintenance = 2,         // Bakımda
    Full = 3,                // Dolu
    Closed = 4               // Kapalı
}