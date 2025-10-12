namespace UniversityMS.Domain.Enums;

/// <summary>
/// Ziyaretçi Tipi
/// </summary>
public enum VisitorType
{
    Guest = 1,                // Misafir
    Contractor = 2,           // Yüklenici
    Vendor = 3,               // Tedarikçi
    Parent = 4,               // Veli
    Interviewer = 5,          // Görüşmeci
    Delivery = 6,             // Kargo/Teslimat
    Maintenance = 7,          // Bakım personeli
    Inspector = 8,            // Denetçi
    Media = 9,                // Basın
    VIP = 10,                 // VIP
    Other = 99                // Diğer
}