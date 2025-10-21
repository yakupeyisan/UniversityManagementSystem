namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kafeteria Bildirim Türü
/// </summary>
public enum CafeteriaNotificationType
{
    /// <summary>
    /// Bakiye Düşük - Bakiye minimum limitin altına düştü
    /// </summary>
    LowBalance = 1,

    /// <summary>
    /// Kart Süresi Doluyor - Geçerlilik süresi 1 ay kaldı
    /// </summary>
    CardExpiring = 2,

    /// <summary>
    /// Kart Engellendi - Kart sistem tarafından engellendi
    /// </summary>
    CardBlocked = 3,

    /// <summary>
    /// Bakiye Yüklendi - Para yükleme işlemi başarılı
    /// </summary>
    BalanceLoaded = 4,

    /// <summary>
    /// Geri Ödeme - Para iade edildi
    /// </summary>
    Refund = 5,

    /// <summary>
    /// Kart İptal - Kart iptal edildi
    /// </summary>
    CardCancelled = 6,

    /// <summary>
    /// Promosyon - Promosyon veya bonus bilgisi
    /// </summary>
    Promotion = 7,

    /// <summary>
    /// Uyarı - Sistemsel uyarı
    /// </summary>
    Warning = 8,

    /// <summary>
    /// Abonelik Başladı - Abonelik aktifleştirildi
    /// </summary>
    SubscriptionStarted = 9,

    /// <summary>
    /// Abonelik Süresi Dolmak Üzere - Abonelik 7 gün sonra bitiyor
    /// </summary>
    SubscriptionExpiring = 10,

    /// <summary>
    /// Abonelik Ended - Abonelik bitmiş
    /// </summary>
    SubscriptionEnded = 11
}