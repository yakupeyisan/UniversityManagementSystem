namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ekipman Durumu
/// </summary>
public enum EquipmentStatus
{
    Available = 1,            // Müsait
    InUse = 2,                // Kullanımda
    Reserved = 3,             // Rezerve
    UnderMaintenance = 4,     // Bakımda
    OutOfService = 5,         // Hizmet dışı
    Damaged = 6,              // Hasarlı
    Retired = 7               // Emekliye ayrılmış
}