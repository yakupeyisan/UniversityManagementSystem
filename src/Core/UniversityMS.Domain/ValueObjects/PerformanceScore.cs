using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Performans Skoru Value Object
/// Skor ve Rating'i birlikte yönetir
/// </summary>
public class PerformanceScore : ValueObject
{
    public decimal Score { get; private set; }
    public PerformanceRating Rating { get; private set; }
    public string RatingDescription { get; private set; } = null!;
    public string ScoreRange { get; private set; } = null!;

    private PerformanceScore() { }

    private PerformanceScore(decimal score)
    {
        Score = Math.Round(score, 2);
        Rating = CalculateRating(score);
        RatingDescription = GetRatingDescription(Rating);
        ScoreRange = GetScoreRange(Rating);
    }

    /// <summary>
    /// Skor değerinden PerformanceScore oluşturur
    /// </summary>
    public static PerformanceScore Create(decimal score)
    {
        if (score < 0 || score > 100)
            throw new DomainException("Performans skoru 0-100 arasında olmalıdır.");

        return new PerformanceScore(score);
    }

    /// <summary>
    /// Rating'den PerformanceScore oluşturur (ortalama skor ile)
    /// </summary>
    public static PerformanceScore FromRating(PerformanceRating rating)
    {
        var averageScore = rating switch
        {
            PerformanceRating.Outstanding => 95m,
            PerformanceRating.ExceedsExpectations => 85m,
            PerformanceRating.MeetsExpectations => 75m,
            PerformanceRating.NeedsImprovement => 65m,
            PerformanceRating.Unsatisfactory => 50m,
            _ => throw new DomainException("Geçersiz performans derecesi.")
        };

        return new PerformanceScore(averageScore);
    }

    /// <summary>
    /// Birden fazla skorun ortalamasından oluşturur
    /// </summary>
    public static PerformanceScore FromAverage(params decimal[] scores)
    {
        if (scores == null || scores.Length == 0)
            throw new DomainException("En az bir skor belirtilmelidir.");

        var average = scores.Average();
        return Create(average);
    }

    /// <summary>
    /// Ağırlıklı ortalamadan oluşturur
    /// </summary>
    public static PerformanceScore FromWeightedAverage(Dictionary<decimal, decimal> scoreWeights)
    {
        if (scoreWeights == null || scoreWeights.Count == 0)
            throw new DomainException("En az bir skor ve ağırlık belirtilmelidir.");

        var totalWeight = scoreWeights.Values.Sum();
        if (Math.Abs(totalWeight - 100) > 0.01m)
            throw new DomainException("Ağırlıkların toplamı 100 olmalıdır.");

        var weightedSum = scoreWeights.Sum(kv => kv.Key * (kv.Value / 100));
        return Create(weightedSum);
    }

    /// <summary>
    /// Rating değerini döndürür (GetRating metodu - geriye dönük uyumluluk için)
    /// </summary>
    public PerformanceRating GetRating() => Rating;

    private static PerformanceRating CalculateRating(decimal score)
    {
        return score switch
        {
            >= 90 => PerformanceRating.Outstanding,
            >= 80 => PerformanceRating.ExceedsExpectations,
            >= 70 => PerformanceRating.MeetsExpectations,
            >= 60 => PerformanceRating.NeedsImprovement,
            _ => PerformanceRating.Unsatisfactory
        };
    }

    private static string GetRatingDescription(PerformanceRating rating)
    {
        return rating switch
        {
            PerformanceRating.Outstanding => "Mükemmel",
            PerformanceRating.ExceedsExpectations => "Beklentilerin Üstünde",
            PerformanceRating.MeetsExpectations => "Beklentileri Karşılıyor",
            PerformanceRating.NeedsImprovement => "Geliştirilmeli",
            PerformanceRating.Unsatisfactory => "Yetersiz",
            _ => "Bilinmiyor"
        };
    }

    private static string GetScoreRange(PerformanceRating rating)
    {
        return rating switch
        {
            PerformanceRating.Outstanding => "90-100",
            PerformanceRating.ExceedsExpectations => "80-89",
            PerformanceRating.MeetsExpectations => "70-79",
            PerformanceRating.NeedsImprovement => "60-69",
            PerformanceRating.Unsatisfactory => "0-59",
            _ => "N/A"
        };
    }

    #region Comparison Operators

    /// <summary>
    /// PerformanceScore ile int karşılaştırması için >= operatörü
    /// </summary>
    public static bool operator >=(PerformanceScore score, int value)
    {
        return score?.Score >= value;
    }

    /// <summary>
    /// PerformanceScore ile int karşılaştırması için <= operatörü
    /// </summary>
    public static bool operator <=(PerformanceScore score, int value)
    {
        return score?.Score <= value;
    }

    /// <summary>
    /// PerformanceScore ile int karşılaştırması için > operatörü
    /// </summary>
    public static bool operator >(PerformanceScore score, int value)
    {
        return score?.Score > value;
    }

    /// <summary>
    /// PerformanceScore ile int karşılaştırması için < operatörü
    /// </summary>
    public static bool operator <(PerformanceScore score, int value)
    {
        return score?.Score < value;
    }

    /// <summary>
    /// PerformanceScore'lar arası >= operatörü
    /// </summary>
    public static bool operator >=(PerformanceScore left, PerformanceScore right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Score >= right.Score;
    }

    /// <summary>
    /// PerformanceScore'lar arası <= operatörü
    /// </summary>
    public static bool operator <=(PerformanceScore left, PerformanceScore right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;
        return left.Score <= right.Score;
    }

    /// <summary>
    /// PerformanceScore'lar arası > operatörü
    /// </summary>
    public static bool operator >(PerformanceScore left, PerformanceScore right)
    {
        if (left is null) return false;
        if (right is null) return true;
        return left.Score > right.Score;
    }

    /// <summary>
    /// PerformanceScore'lar arası < operatörü
    /// </summary>
    public static bool operator <(PerformanceScore left, PerformanceScore right)
    {
        if (left is null) return right is not null;
        if (right is null) return false;
        return left.Score < right.Score;
    }

    #endregion

    #region Helper Methods

    /// <summary>
    /// Mükemmel performans mı?
    /// </summary>
    public bool IsExcellent() => Rating == PerformanceRating.Outstanding;

    /// <summary>
    /// İyi performans mı? (Beklentileri karşılıyor veya üstünde)
    /// </summary>
    public bool IsGood() => Rating >= PerformanceRating.MeetsExpectations;

    /// <summary>
    /// Geliştirilmesi gerekiyor mu?
    /// </summary>
    public bool NeedsImprovement() => Rating <= PerformanceRating.NeedsImprovement;

    /// <summary>
    /// Yetersiz mi?
    /// </summary>
    public bool IsUnsatisfactory() => Rating == PerformanceRating.Unsatisfactory;

    /// <summary>
    /// Geçer not mu? (70 ve üzeri)
    /// </summary>
    public bool IsPassing() => Score >= 70;

    /// <summary>
    /// Başka bir skorla karşılaştır
    /// </summary>
    public bool IsBetterThan(PerformanceScore other) => Score > other.Score;

    /// <summary>
    /// Yüzdelik dilim hesapla (0-100 arası norm için)
    /// </summary>
    public int GetPercentile() => (int)Score;

    /// <summary>
    /// Rating'in numerik değerini al (1-5)
    /// </summary>
    public int GetNumericRating() => (int)Rating;

    #endregion

    #region Arithmetic Operations

    /// <summary>
    /// İki skorun ortalamasını al
    /// </summary>
    public static PerformanceScore Average(PerformanceScore score1, PerformanceScore score2)
    {
        var avg = (score1.Score + score2.Score) / 2;
        return Create(avg);
    }

    /// <summary>
    /// Skoru güncelle (yeni değerlendirme)
    /// </summary>
    public PerformanceScore UpdateScore(decimal newScore)
    {
        return Create(newScore);
    }

    /// <summary>
    /// İki skorun ağırlıklı ortalaması
    /// </summary>
    public static PerformanceScore WeightedAverage(
        PerformanceScore score1, decimal weight1,
        PerformanceScore score2, decimal weight2)
    {
        if (weight1 + weight2 != 100)
            throw new DomainException("Ağırlıkların toplamı 100 olmalıdır.");

        var weighted = (score1.Score * weight1 + score2.Score * weight2) / 100;
        return Create(weighted);
    }

    #endregion

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Score;
        yield return Rating;
    }

    public override string ToString() => $"{Score:F2} - {RatingDescription} ({ScoreRange})";
}