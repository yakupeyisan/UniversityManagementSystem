namespace UniversityMS.Domain.Enums;

/// <summary>
/// Performans Değerlendirme Derecelendirmesi Enum
/// </summary>
public enum PerformanceRating
{
    Unsatisfactory = 1,        // Yetersiz (0-59)
    NeedsImprovement = 2,      // Geliştirilmeli (60-69)
    MeetsExpectations = 3,     // Beklentileri karşılıyor (70-79)
    ExceedsExpectations = 4,   // Beklentilerin üstünde (80-89)
    Outstanding = 5            // Mükemmel (90-100)
}