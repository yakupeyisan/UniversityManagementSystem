namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kafeteria İşlem Türü
/// </summary>
public enum CafeteriaTransactionType
{
    /// <summary>
    /// Bakiye Yükleme - Öğrenci/Personel para yükledi
    /// </summary>
    Credit = 1,

    /// <summary>
    /// Harcama - Kafeterya menüsünden satın alma
    /// </summary>
    Debit = 2,

    /// <summary>
    /// Geri Ödeme - Hatalı işlem, iade, vs
    /// </summary>
    Refund = 3,

    /// <summary>
    /// İdari Ayarlama - Yönetici manuel ayarlama
    /// </summary>
    Adjustment = 4,

    /// <summary>
    /// Bonus - Promosyon, ödül, vs
    /// </summary>
    Bonus = 5,

    /// <summary>
    /// Ceza - Disiplin cezası
    /// </summary>
    Penalty = 6,

    /// <summary>
    /// Devir - Kişi tarafından kişiye transfer
    /// </summary>
    Transfer = 7,

    /// <summary>
    /// Abonelik Ücretlendirmesi - Aylık/Haftalık abonelik ücreti
    /// </summary>
    SubscriptionCharge = 8,

    /// <summary>
    /// Abonelik İadesi - Abonelik iadesi
    /// </summary>
    SubscriptionRefund = 9
}