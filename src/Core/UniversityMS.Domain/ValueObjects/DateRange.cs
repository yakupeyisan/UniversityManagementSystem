using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Tarih Aralığı - Value Object
/// Başlangıç ve bitiş tarihleri arasındaki dönem
/// </summary>
public class DateRange
{
    public Guid Id { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DateRange() { }

    private DateRange(DateTime startDate, DateTime endDate, string name)
    {
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        Name = name;
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static DateRange Create(DateTime startDate, DateTime endDate, string name)
    {
        if (startDate > endDate)
            throw new ArgumentException("Başlangıç tarihi bitiş tarihinden önce olmalı.");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ad boş olamaz.");

        return new DateRange(startDate, endDate, name);
    }

    /// <summary>
    /// Verilen tarih bu aralıkta mı?
    /// </summary>
    public bool IsInRange(DateTime date)
    {
        var dateOnly = date.Date;
        return dateOnly >= StartDate && dateOnly <= EndDate;
    }

    /// <summary>
    /// Kaç gün sürecek?
    /// </summary>
    public int GetDayCount()
    {
        return (int)(EndDate - StartDate).TotalDays + 1;
    }

    /// <summary>
    /// Aralıkların çakışıp çakışmadığını kontrol et
    /// </summary>
    public bool Overlaps(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    public override string ToString()
    {
        return $"{Name} ({StartDate:d} - {EndDate:d})";
    }
}