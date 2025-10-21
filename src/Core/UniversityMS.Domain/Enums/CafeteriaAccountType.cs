namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kafeteria Hesabı Türü - Bir kişi her tür için bir kartı olabilir
/// Örn: Ali STUDENT olarak kart alabilir, aynı zamanda STAFF olarak başka kart
/// </summary>
public enum CafeteriaAccountType
{
    /// <summary>
    /// Öğrenci Kartı - Lisans, YL, Doktora öğrencileri
    /// Fiyatlandırma: Öğrenci tarifesi (indirimli)
    /// </summary>
    Student = 1,

    /// <summary>
    /// Akademik Personel Kartı - Öğretim Üyeleri
    /// Fiyatlandırma: Personel tarifesi
    /// </summary>
    AcademicStaff = 2,

    /// <summary>
    /// İdari Personel Kartı - İdari çalışanlar
    /// Fiyatlandırma: Personel tarifesi
    /// </summary>
    AdministrativeStaff = 3,

    /// <summary>
    /// Güvenlik Görevlisi Kartı
    /// Fiyatlandırma: Personel tarifesi
    /// </summary>
    SecurityPersonnel = 4,

    /// <summary>
    /// Yardımcı Personel Kartı - Temizlik, bakım, vs
    /// Fiyatlandırma: Personel tarifesi
    /// </summary>
    SupportStaff = 5,

    /// <summary>
    /// Yemekhane Personeli Kartı
    /// Fiyatlandırma: Özel - Personel + indirim
    /// </summary>
    CafeteriaStaff = 6,

    /// <summary>
    /// Konuk/Misafir Kartı - Konuk öğretim üyeleri, stajyerler
    /// Fiyatlandırma: Misafir tarifesi (normal fiyat)
    /// </summary>
    Guest = 7,

    /// <summary>
    /// Ziyaretçi Kartı - Geçici ziyaretçiler
    /// Fiyatlandırma: Ziyaretçi tarifesi (yüksek)
    /// </summary>
    Visitor = 8
}