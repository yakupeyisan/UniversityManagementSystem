using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

public sealed class DateRange : ValueObject
{
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    private DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public static DateRange Create(DateTime startDate, DateTime endDate)
    {
        if (startDate > endDate)
            throw new DomainException("Başlangıç tarihi bitiş tarihinden sonra olamaz.");

        return new DateRange(startDate.Date, endDate.Date);
    }

    public int GetDurationInDays()
    {
        return (EndDate - StartDate).Days + 1;
    }

    public bool Contains(DateTime date)
    {
        var checkDate = date.Date;
        return checkDate >= StartDate && checkDate <= EndDate;
    }

    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return StartDate;
        yield return EndDate;
    }

    public override string ToString() => $"{StartDate:dd.MM.yyyy} - {EndDate:dd.MM.yyyy}";
}