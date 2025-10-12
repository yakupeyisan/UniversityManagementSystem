using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Çalışma Saatleri Value Object
/// </summary>
public class WorkingHours : ValueObject
{
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public int WeeklyHours { get; private set; }
    public int DailyHours { get; private set; }

    private WorkingHours() { }

    private WorkingHours(TimeOnly startTime, TimeOnly endTime, int weeklyHours)
    {
        StartTime = startTime;
        EndTime = endTime;
        WeeklyHours = weeklyHours;
        DailyHours = (int)(endTime - startTime).TotalHours;
    }

    public static WorkingHours Create(TimeOnly startTime, TimeOnly endTime, int weeklyHours = 40)
    {
        if (endTime <= startTime)
            throw new DomainException("Bitiş saati başlangıç saatinden sonra olmalıdır.");

        if (weeklyHours <= 0 || weeklyHours > 168)
            throw new DomainException("Haftalık çalışma saati 1-168 arasında olmalıdır.");

        return new WorkingHours(startTime, endTime, weeklyHours);
    }

    public static WorkingHours CreateStandard()
    {
        // Standart: 09:00-18:00, 40 saat/hafta
        return Create(new TimeOnly(9, 0), new TimeOnly(18, 0), 40);
    }

    public static WorkingHours CreateFlexible(int weeklyHours = 40)
    {
        // Esnek çalışma saatleri
        return Create(new TimeOnly(0, 0), new TimeOnly(23, 59), weeklyHours);
    }

    public bool IsFlexible()
    {
        return StartTime == new TimeOnly(0, 0) && EndTime == new TimeOnly(23, 59);
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartTime;
        yield return EndTime;
        yield return WeeklyHours;
    }

    public override string ToString() => IsFlexible()
        ? $"Esnek ({WeeklyHours} saat/hafta)"
        : $"{StartTime:HH:mm}-{EndTime:HH:mm} ({WeeklyHours} saat/hafta)";
}