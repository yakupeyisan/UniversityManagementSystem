namespace UniversityMS.Domain.Enums;

/// <summary>
/// Kafeteria Yemek Planı - Account type'a göre farklı planlar
/// Örn: Student için 'Student Standard', Staff için 'Staff Premium'
/// </summary>
public enum MealPlanType
{
    /// <summary>
    /// Standart Plan - Temel menü seçeneği
    /// </summary>
    Standard = 1,

    /// <summary>
    /// Premium Plan - Daha fazla seçenek ve daha kaliteli yemekler
    /// </summary>
    Premium = 2,

    /// <summary>
    /// VIP Plan - Ekstra özel hizmetler, öncelik, vs
    /// </summary>
    VIP = 3,

    /// <summary>
    /// Özel Diyet Planı - Vejetaryen, vegan, gluten-free, vs
    /// </summary>
    SpecialDiet = 4,

    /// <summary>
    /// Aylık Abonelik - Sabit ücretli aylık abonelik (sınırsız)
    /// </summary>
    MonthlySubscription = 5,

    /// <summary>
    /// Haftalık Abonelik - Sabit ücretli haftalık abonelik
    /// </summary>
    WeeklySubscription = 6,

    /// <summary>
    /// Hiçbiri - Plan yok, sadece bakiye ile ödeme
    /// </summary>
    NoSubscription = 7
}