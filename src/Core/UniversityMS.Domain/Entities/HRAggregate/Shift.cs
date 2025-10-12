using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.HRAggregate;

/// <summary>
/// Vardiya Entity
/// </summary>
public class Shift : AuditableEntity
{
    public Guid EmployeeId { get; private set; }
    public DateOnly Date { get; private set; }
    public TimeOnly StartTime { get; private set; }
    public TimeOnly EndTime { get; private set; }
    public ShiftPattern ShiftPattern { get; private set; }
    public ShiftStatus Status { get; private set; }
    public decimal? OvertimeHours { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Property
    public Employee Employee { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private Shift() { }

    private Shift(
        Guid employeeId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        ShiftPattern shiftPattern,
        string? notes = null)
    {
        EmployeeId = employeeId;
        Date = date;
        StartTime = startTime;
        EndTime = endTime;
        ShiftPattern = shiftPattern;
        Status = ShiftStatus.Scheduled;
        Notes = notes;
    }

    public static Shift Create(
        Guid employeeId,
        DateOnly date,
        TimeOnly startTime,
        TimeOnly endTime,
        ShiftPattern shiftPattern,
        string? notes = null)
    {
        if (date < DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Vardiya tarihi geçmiş olamaz.");

        if (endTime <= startTime)
            throw new DomainException("Bitiş saati başlangıç saatinden büyük olmalıdır.");

        return new Shift(employeeId, date, startTime, endTime, shiftPattern, notes);
    }

    public void Start()
    {
        if (Status != ShiftStatus.Scheduled)
            throw new DomainException("Sadece planlanmış vardiyalar başlatılabilir.");

        if (Date > DateOnly.FromDateTime(DateTime.UtcNow))
            throw new DomainException("Gelecek tarihli vardiya başlatılamaz.");

        Status = ShiftStatus.InProgress;
    }

    public void Complete(decimal? overtimeHours = null)
    {
        if (Status != ShiftStatus.InProgress)
            throw new DomainException("Sadece devam eden vardiyalar tamamlanabilir.");

        if (overtimeHours.HasValue && overtimeHours < 0)
            throw new DomainException("Fazla mesai saati negatif olamaz.");

        Status = ShiftStatus.Completed;
        OvertimeHours = overtimeHours;
    }

    public void Cancel(string reason)
    {
        if (Status == ShiftStatus.Completed)
            throw new DomainException("Tamamlanmış vardiya iptal edilemez.");

        Status = ShiftStatus.Cancelled;
        Notes = $"İptal sebebi: {reason}";
    }

    public void Modify(TimeOnly newStartTime, TimeOnly newEndTime)
    {
        if (Status != ShiftStatus.Scheduled)
            throw new DomainException("Sadece planlanmış vardiyalar değiştirilebilir.");

        if (newEndTime <= newStartTime)
            throw new DomainException("Bitiş saati başlangıç saatinden büyük olmalıdır.");

        StartTime = newStartTime;
        EndTime = newEndTime;
        Status = ShiftStatus.Modified;
    }

    public decimal GetTotalHours()
    {
        return (decimal)(EndTime - StartTime).TotalHours;
    }

    public decimal GetTotalHoursWithOvertime()
    {
        return GetTotalHours() + (OvertimeHours ?? 0);
    }

    public bool IsNightShift()
    {
        return ShiftPattern == ShiftPattern.Night ||
               StartTime.Hour >= 22 ||
               EndTime.Hour <= 6;
    }
}