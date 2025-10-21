namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kafeteria Kartı Durumu
/// </summary>
public enum CafeteriaCardStatus
{
    /// <summary>
    /// İnaktif - Henüz aktivasyonu bekleniyor
    /// </summary>
    Inactive = 0,

    /// <summary>
    /// Aktif - Kartı kullanmaya hazır
    /// </summary>
    Active = 1,

    /// <summary>
    /// Geçici olarak durdurulmuş - Borç var, çok harcama, vs
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Engellendi - Sistem tarafından otomatik engelleme
    /// (Şüpheli aktivite, tekrarlayan hatalar, vs)
    /// </summary>
    Blocked = 3,

    /// <summary>
    /// Yönetici tarafından engellendi - El ile engelleme
    /// (Disiplin cezası, kaybedildi, çalındı, vs)
    /// </summary>
    AdminBlocked = 4,

    /// <summary>
    /// Süresi dolmuş - Geçerlilik süresi bitti
    /// </summary>
    Expired = 5,

    /// <summary>
    /// İptal edildi - Kullanıcı tarafından iptal ettirdi
    /// </summary>
    Cancelled = 6,

    /// <summary>
    /// Kaybedildi - Kart kayboldu, değiştirmek gerekli
    /// </summary>
    Lost = 7,

    /// <summary>
    /// Çalındı - Kart çalındı, acil engelleme gerekli
    /// </summary>
    Stolen = 8
}