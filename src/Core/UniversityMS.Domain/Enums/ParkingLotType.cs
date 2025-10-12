namespace UniversityMS.Domain.Enums;

/// <summary>
/// Otopark Tipi
/// </summary>
public enum ParkingLotType
{
    Surface = 1,              // Açık otopark
    MultiLevel = 2,           // Katlı otopark
    Underground = 3,          // Yeraltı otoparkı
    Reserved = 4,             // Ayrılmış otopark
    Visitor = 5,              // Ziyaretçi otoparkı
    Staff = 6,                // Personel otoparkı
    Student = 7,              // Öğrenci otoparkı
    Motorcycle = 8,           // Motosiklet otoparkı
    Bicycle = 9,              // Bisiklet park yeri
    Disabled = 10             // Engelli otoparkı
}