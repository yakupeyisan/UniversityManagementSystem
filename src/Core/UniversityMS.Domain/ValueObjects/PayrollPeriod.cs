using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Bordro Dönemi Value Object
/// </summary>
public class PayrollPeriod : ValueObject
{
    public int Year { get; private set; }
    public int Month { get; private set; }
    public string PeriodName { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private PayrollPeriod() { }

    private PayrollPeriod(int year, int month)
    {
        Year = year;
        Month = month;
        PeriodName = $"{year}-{month:D2}";
        StartDate = new DateTime(year, month, 1);
        EndDate = StartDate.AddMonths(1).AddDays(-1);
    }

    public static PayrollPeriod Create(int year, int month)
    {
        if (year < 2000 || year > 2100)
            throw new DomainException("Geçerli bir yıl giriniz.");

        if (month < 1 || month > 12)
            throw new DomainException("Ay 1-12 arasında olmalıdır.");

        return new PayrollPeriod(year, month);
    }

    public static PayrollPeriod Current()
    {
        var now = DateTime.UtcNow;
        return Create(now.Year, now.Month);
    }

    public PayrollPeriod Previous()
    {
        var previousMonth = StartDate.AddMonths(-1);
        return Create(previousMonth.Year, previousMonth.Month);
    }

    public PayrollPeriod Next()
    {
        var nextMonth = StartDate.AddMonths(1);
        return Create(nextMonth.Year, nextMonth.Month);
    }

    public bool IsCurrent()
    {
        var now = DateTime.UtcNow;
        return Year == now.Year && Month == now.Month;
    }

    public bool IsPast()
    {
        var now = DateTime.UtcNow;
        return Year < now.Year || (Year == now.Year && Month < now.Month);
    }

    public int GetWorkingDays()
    {
        int workingDays = 0;
        for (var date = StartDate; date <= EndDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                workingDays++;
        }
        return workingDays;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Year;
        yield return Month;
    }

    public override string ToString() => PeriodName;
}