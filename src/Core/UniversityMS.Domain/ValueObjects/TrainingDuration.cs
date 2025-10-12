using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Eğitim Süresi Value Object
/// </summary>
public class TrainingDuration : ValueObject
{
    public int Hours { get; private set; }
    public int Days { get; private set; }

    private TrainingDuration() { }

    private TrainingDuration(int hours, int days)
    {
        Hours = hours;
        Days = days;
    }

    public static TrainingDuration Create(int hours, int days = 0)
    {
        if (hours < 0)
            throw new DomainException("Eğitim saati negatif olamaz.");

        if (days < 0)
            throw new DomainException("Eğitim günü negatif olamaz.");

        return new TrainingDuration(hours, days);
    }

    public static TrainingDuration FromHours(int totalHours)
    {
        if (totalHours < 0)
            throw new DomainException("Toplam saat negatif olamaz.");

        var days = totalHours / 8;  // 8 saatlik iş günü
        var hours = totalHours % 8;

        return new TrainingDuration(hours, days);
    }

    public int GetTotalHours()
    {
        return (Days * 8) + Hours;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hours;
        yield return Days;
    }

    public override string ToString()
    {
        if (Days > 0)
            return $"{Days} gün {Hours} saat";
        return $"{Hours} saat";
    }
}