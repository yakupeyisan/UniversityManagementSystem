namespace UniversityMS.Domain.Enums.Extensions;

/// <summary>
/// CafeteriaAccountType Extension Methods
/// </summary>
public static class CafeteriaAccountTypeExtensions
{
    /// <summary>
    /// Hesap türü için varsayılan fiyatlandırma seviyesi
    /// </summary>
    public static decimal GetPricingMultiplier(this CafeteriaAccountType type)
    {
        return type switch
        {
            CafeteriaAccountType.Student => 0.80m,           // %20 indirim
            CafeteriaAccountType.AcademicStaff => 1.0m,      // Normal fiyat
            CafeteriaAccountType.AdministrativeStaff => 1.0m, // Normal fiyat
            CafeteriaAccountType.SecurityPersonnel => 0.85m,  // %15 indirim
            CafeteriaAccountType.SupportStaff => 0.85m,       // %15 indirim
            CafeteriaAccountType.CafeteriaStaff => 0.50m,     // %50 indirim
            CafeteriaAccountType.Guest => 1.2m,              // %20 ek ücret
            CafeteriaAccountType.Visitor => 1.3m,            // %30 ek ücret
            _ => 1.0m
        };
    }

    /// <summary>
    /// Hesap türü için önerilen meal plan'lar
    /// </summary>
    public static List<MealPlanType> GetAvailablePlans(this CafeteriaAccountType type)
    {
        return type switch
        {
            CafeteriaAccountType.Student => new()
            {
                MealPlanType.Standard,
                MealPlanType.NoSubscription,
                MealPlanType.WeeklySubscription,
                MealPlanType.SpecialDiet
            },
            CafeteriaAccountType.AcademicStaff => new()
            {
                MealPlanType.Standard,
                MealPlanType.Premium,
                MealPlanType.MonthlySubscription,
                MealPlanType.SpecialDiet,
                MealPlanType.NoSubscription
            },
            CafeteriaAccountType.AdministrativeStaff => new()
            {
                MealPlanType.Standard,
                MealPlanType.Premium,
                MealPlanType.MonthlySubscription,
                MealPlanType.SpecialDiet,
                MealPlanType.NoSubscription
            },
            CafeteriaAccountType.SecurityPersonnel => new()
            {
                MealPlanType.Standard,
                MealPlanType.NoSubscription
            },
            CafeteriaAccountType.SupportStaff => new()
            {
                MealPlanType.Standard,
                MealPlanType.NoSubscription
            },
            CafeteriaAccountType.CafeteriaStaff => new()
            {
                MealPlanType.MonthlySubscription,
                MealPlanType.SpecialDiet
            },
            CafeteriaAccountType.Guest => new()
            {
                MealPlanType.Standard,
                MealPlanType.NoSubscription
            },
            CafeteriaAccountType.Visitor => new()
            {
                MealPlanType.NoSubscription
            },
            _ => new() { MealPlanType.Standard, MealPlanType.NoSubscription }
        };
    }

    /// <summary>
    /// Hesap türü açıklaması
    /// </summary>
    public static string GetDescription(this CafeteriaAccountType type)
    {
        return type switch
        {
            CafeteriaAccountType.Student => "Öğrenci Kartı",
            CafeteriaAccountType.AcademicStaff => "Akademik Personel Kartı",
            CafeteriaAccountType.AdministrativeStaff => "İdari Personel Kartı",
            CafeteriaAccountType.SecurityPersonnel => "Güvenlik Görevlisi Kartı",
            CafeteriaAccountType.SupportStaff => "Yardımcı Personel Kartı",
            CafeteriaAccountType.CafeteriaStaff => "Yemekhane Personeli Kartı",
            CafeteriaAccountType.Guest => "Konuk Kartı",
            CafeteriaAccountType.Visitor => "Ziyaretçi Kartı",
            _ => "Bilinmeyen Kart Türü"
        };
    }
}