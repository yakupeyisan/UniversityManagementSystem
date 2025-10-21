namespace UniversityMS.Domain.Enums;

/// <summary>
/// Beslenme Kısıtlaması - Özel diyet gereksinimleri
/// </summary>
public enum DietaryRestriction
{
    /// <summary>
    /// Hiçbiri - Kısıtlama yok
    /// </summary>
    None = 0,

    /// <summary>
    /// Vejetaryen
    /// </summary>
    Vegetarian = 1,

    /// <summary>
    /// Vegan
    /// </summary>
    Vegan = 2,

    /// <summary>
    /// Glutensiz (Celiac)
    /// </summary>
    GlutenFree = 3,

    /// <summary>
    /// Balık yok
    /// </summary>
    NoSeafood = 4,

    /// <summary>
    /// Tavuk yok
    /// </summary>
    NoPoultry = 5,

    /// <summary>
    /// Süt ürünü yok
    /// </summary>
    DairyFree = 6,

    /// <summary>
    /// Fındık alerjisi
    /// </summary>
    NutAllergy = 7,

    /// <summary>
    /// Yumurta alerjisi
    /// </summary>
    EggAllergy = 8,

    /// <summary>
    /// Çoklu alerjiler
    /// </summary>
    MultipleAllergies = 9
}