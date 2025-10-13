using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.ValueObjects;

/// <summary>
/// Zaman dilimi Value Object (örn: 09:00-09:50)
/// </summary>
public class TimeSlot : IEquatable<TimeSlot>
{
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public int DurationMinutes => (int)(EndTime - StartTime).TotalMinutes;

    private TimeSlot() { } // EF Core

    private TimeSlot(TimeSpan startTime, TimeSpan endTime)
    {
        StartTime = startTime;
        EndTime = endTime;
    }

    public static TimeSlot Create(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
            throw new DomainException("Başlangıç saati bitiş saatinden önce olmalıdır.");

        if (endTime - startTime < TimeSpan.FromMinutes(40))
            throw new DomainException("Ders süresi en az 40 dakika olmalıdır.");

        if (endTime - startTime > TimeSpan.FromMinutes(240))
            throw new DomainException("Ders süresi en fazla 4 saat olabilir.");

        return new TimeSlot(startTime, endTime);
    }

    public static TimeSlot Create(string startTime, string endTime)
    {
        if (!TimeSpan.TryParse(startTime, out var start))
            throw new DomainException("Geçersiz başlangıç saati formatı.");

        if (!TimeSpan.TryParse(endTime, out var end))
            throw new DomainException("Geçersiz bitiş saati formatı.");

        return Create(start, end);
    }

    /// <summary>
    /// İki zaman diliminin çakışıp çakışmadığını kontrol eder
    /// </summary>
    public bool ConflictsWith(TimeSlot other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }

    public bool IsWithinRange(TimeSlot range)
    {
        return StartTime >= range.StartTime && EndTime <= range.EndTime;
    }

    public override string ToString()
    {
        return $"{StartTime:hh\\:mm}-{EndTime:hh\\:mm}";
    }

    // IEquatable implementation
    public bool Equals(TimeSlot? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return StartTime == other.StartTime && EndTime == other.EndTime;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as TimeSlot);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartTime, EndTime);
    }

    public static bool operator ==(TimeSlot? left, TimeSlot? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(TimeSlot? left, TimeSlot? right)
    {
        return !Equals(left, right);
    }
}