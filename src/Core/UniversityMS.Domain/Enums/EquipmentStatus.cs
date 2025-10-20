namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ekipman Durumu - NEW
/// </summary>
public enum EquipmentStatus
{
    Operational = 0,         // Operasyonel
    NeedsRepair = 1,         // Onarım gerekli
    InRepair = 2,            // Onarımda
    OutOfService = 3,        // Hizmet dışı
    Decommissioned = 4,      // Hizmetinden çekildi
    Calibrating = 5,         // Kalibrasyon yapılıyor
    Reserved = 6             // Rezerve
}