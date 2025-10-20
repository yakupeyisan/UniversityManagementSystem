namespace UniversityMS.Domain.Enums;

/// <summary>
/// Erişim Seviyesi - NEW
/// </summary>
public enum AccessLevel
{
    Guest = 0,               // Ziyaretçi
    Student = 1,             // Öğrenci
    Staff = 2,               // Personel
    Instructor = 3,          // Öğretim görevlisi
    Manager = 4,             // Müdür
    Admin = 5,               // Yönetici
    SuperAdmin = 6           // Süper yönetici
}