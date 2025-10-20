namespace UniversityMS.Domain.Enums;

/// <summary>
/// Depo Türü - Mevcut (Tutuldu)
/// </summary>
public enum WarehouseType
{
    Main = 0,                // Ana
    Department = 1,          // Birim
    Laboratory = 2,          // Laboratuvar
    IT = 3,                  // IT
    Maintenance = 4,         // Bakım
    Archive = 5,             // Arşiv
    Cold = 6,                // Soğuk
    Hazardous = 7            // Tehlikeli
}