namespace UniversityMS.Domain.Enums.Extensions;

/// <summary>
/// CafeteriaCardStatus Extension Methods
/// </summary>
public static class CafeteriaCardStatusExtensions
{
    /// <summary>
    /// Kartın kullanılabilir mi kontrol et
    /// </summary>
    public static bool IsUsable(this CafeteriaCardStatus status)
    {
        return status == CafeteriaCardStatus.Active;
    }

    /// <summary>
    /// Kartın engellendi mi kontrol et
    /// </summary>
    public static bool IsBlocked(this CafeteriaCardStatus status)
    {
        return status is CafeteriaCardStatus.Blocked
            or CafeteriaCardStatus.AdminBlocked
            or CafeteriaCardStatus.Lost
            or CafeteriaCardStatus.Stolen;
    }

    /// <summary>
    /// Kartın geçerliliği var mı kontrol et
    /// </summary>
    public static bool IsValid(this CafeteriaCardStatus status)
    {
        return status is not (CafeteriaCardStatus.Expired or CafeteriaCardStatus.Cancelled);
    }

    /// <summary>
    /// Kartın durumu açıklamasını al
    /// </summary>
    public static string GetDescription(this CafeteriaCardStatus status)
    {
        return status switch
        {
            CafeteriaCardStatus.Inactive => "İnaktif - Aktivasyonu bekleniyor",
            CafeteriaCardStatus.Active => "Aktif - Kullanılabilir",
            CafeteriaCardStatus.Suspended => "Geçici olarak durdurulmuş",
            CafeteriaCardStatus.Blocked => "Sistem tarafından engellendi",
            CafeteriaCardStatus.AdminBlocked => "Yönetici tarafından engellendi",
            CafeteriaCardStatus.Expired => "Süresi dolmuş",
            CafeteriaCardStatus.Cancelled => "İptal edildi",
            CafeteriaCardStatus.Lost => "Kayboldu",
            CafeteriaCardStatus.Stolen => "Çalındı",
            _ => "Bilinmeyen durum"
        };
    }

    /// <summary>
    /// Kartı Active durumuna geçir
    /// </summary>
    public static bool CanActivate(this CafeteriaCardStatus status)
    {
        return status is CafeteriaCardStatus.Inactive
            or CafeteriaCardStatus.Suspended;
    }

    /// <summary>
    /// Kartı Suspended durumuna geçir
    /// </summary>
    public static bool CanSuspend(this CafeteriaCardStatus status)
    {
        return status == CafeteriaCardStatus.Active;
    }

    /// <summary>
    /// Kartı Block durumuna geçir
    /// </summary>
    public static bool CanBlock(this CafeteriaCardStatus status)
    {
        return status is CafeteriaCardStatus.Active
            or CafeteriaCardStatus.Suspended;
    }

    /// <summary>
    /// Kartı iptal et
    /// </summary>
    public static bool CanCancel(this CafeteriaCardStatus status)
    {
        return status is not (CafeteriaCardStatus.Cancelled or CafeteriaCardStatus.Expired);
    }
}