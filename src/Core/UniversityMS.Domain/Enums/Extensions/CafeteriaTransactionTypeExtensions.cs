namespace UniversityMS.Domain.Enums.Extensions;

/// <summary>
/// CafeteriaTransactionType Extension Methods
/// </summary>
public static class CafeteriaTransactionTypeExtensions
{
    /// <summary>
    /// İşlem türü bakiye artırır mı
    /// </summary>
    public static bool IncreasesBalance(this CafeteriaTransactionType type)
    {
        return type is CafeteriaTransactionType.Credit
            or CafeteriaTransactionType.Refund
            or CafeteriaTransactionType.Bonus
            or CafeteriaTransactionType.SubscriptionRefund;
    }

    /// <summary>
    /// İşlem türü bakiye azaltır mı
    /// </summary>
    public static bool DecreasesBalance(this CafeteriaTransactionType type)
    {
        return type is CafeteriaTransactionType.Debit
            or CafeteriaTransactionType.Penalty
            or CafeteriaTransactionType.SubscriptionCharge;
    }

    /// <summary>
    /// İşlem türü açıklaması
    /// </summary>
    public static string GetDescription(this CafeteriaTransactionType type)
    {
        return type switch
        {
            CafeteriaTransactionType.Credit => "Bakiye Yükleme",
            CafeteriaTransactionType.Debit => "Harcama",
            CafeteriaTransactionType.Refund => "Geri Ödeme",
            CafeteriaTransactionType.Adjustment => "İdari Ayarlama",
            CafeteriaTransactionType.Bonus => "Bonus",
            CafeteriaTransactionType.Penalty => "Ceza",
            CafeteriaTransactionType.Transfer => "Devir",
            CafeteriaTransactionType.SubscriptionCharge => "Abonelik Ücreti",
            CafeteriaTransactionType.SubscriptionRefund => "Abonelik İadesi",
            _ => "Bilinmeyen işlem"
        };
    }
}