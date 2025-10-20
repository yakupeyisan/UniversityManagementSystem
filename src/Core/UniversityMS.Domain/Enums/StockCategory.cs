namespace UniversityMS.Domain.Enums;

/// <summary>
/// Stok Kategorisi - Mevcut (Tutuldu, 1→0)
/// </summary>
public enum StockCategory
{
    OfficeSupplies = 0,      // Ofis malzemeleri
    Cleaning = 1,            // Temizlik
    Laboratory = 2,          // Laboratuvar
    IT = 3,                  // Bilgisayar
    Furniture = 4,           // Mobilya
    Equipment = 5,           // Ekipman
    Stationery = 6,          // Kırtasiye
    Consumables = 7,         // Sarf malzeme
    ChemicalReagents = 8,    // Kimyasal reaktif
    MedicalSupplies = 9,     // Tıbbi malzeme
    Food = 10,               // Gıda
    MaintenanceTools = 11,   // Bakım araçları
    SafetyEquipment = 12,    // Güvenlik ekipmanı
    Other = 99               // Diğer
}