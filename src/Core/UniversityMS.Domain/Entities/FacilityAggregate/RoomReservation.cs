using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.FacilityEvents;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Oda Rezervasyonu (RoomReservation) Entity
/// </summary>
public class RoomReservation : AuditableEntity
{
    public Guid RoomId { get; private set; }
    public Guid ReservedBy { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public RoomReservationStatus Status { get; private set; }
    public string Purpose { get; private set; } = null!;
    public int? ExpectedAttendees { get; private set; }
    public bool RequiresProjector { get; private set; }
    public bool RequiresComputer { get; private set; }
    public bool RequiresWhiteboard { get; private set; }
    public string? SpecialRequirements { get; private set; }
    public DateTime? CheckInTime { get; private set; }
    public DateTime? CheckOutTime { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Room Room { get; private set; } = null!;
    public Person ReservedByPerson { get; private set; } = null!;

    private RoomReservation() { }

    private RoomReservation(
        Guid roomId,
        Guid reservedBy,
        DateTime startTime,
        DateTime endTime,
        string purpose,
        int? expectedAttendees = null)
    {
        RoomId = roomId;
        ReservedBy = reservedBy;
        StartTime = startTime;
        EndTime = endTime;
        Purpose = purpose;
        ExpectedAttendees = expectedAttendees;
        Status = RoomReservationStatus.Pending;
        RequiresProjector = false;
        RequiresComputer = false;
        RequiresWhiteboard = false;
    }

    public static RoomReservation Create(
        Guid roomId,
        Guid reservedBy,
        DateTime startTime,
        DateTime endTime,
        string purpose,
        int? expectedAttendees = null)
    {
        if (endTime <= startTime)
            throw new DomainException("Bitiş zamanı başlangıç zamanından sonra olmalıdır.");

        if (string.IsNullOrWhiteSpace(purpose))
            throw new DomainException("Rezervasyon amacı belirtilmelidir.");

        if (startTime < DateTime.UtcNow)
            throw new DomainException("Geçmiş tarihli rezervasyon yapılamaz.");

        var reservation = new RoomReservation(roomId, reservedBy, startTime, endTime, purpose, expectedAttendees);
        reservation.AddDomainEvent(new RoomReservedEvent(roomId, reservedBy, startTime, endTime, purpose));

        return reservation;
    }

    public void Confirm()
    {
        if (Status != RoomReservationStatus.Pending)
            throw new DomainException("Sadece bekleyen rezervasyonlar onaylanabilir.");

        Status = RoomReservationStatus.Confirmed;
    }

    public void Cancel(string reason)
    {
        if (Status == RoomReservationStatus.Completed)
            throw new DomainException("Tamamlanmış rezervasyon iptal edilemez.");

        Status = RoomReservationStatus.Cancelled;
        Notes = $"İptal sebebi: {reason}";

        AddDomainEvent(new RoomReservationCancelledEvent(Id, RoomId, reason));
    }

    public void CheckIn()
    {
        if (Status != RoomReservationStatus.Confirmed)
            throw new DomainException("Sadece onaylanmış rezervasyonlar için giriş yapılabilir.");

        if (DateTime.UtcNow < StartTime.AddMinutes(-15))
            throw new DomainException("Rezervasyon saatinden 15 dakikadan fazla önce giriş yapılamaz.");

        Status = RoomReservationStatus.InUse;
        CheckInTime = DateTime.UtcNow;
    }

    public void CheckOut()
    {
        if (Status != RoomReservationStatus.InUse)
            throw new DomainException("Sadece kullanımdaki rezervasyonlar için çıkış yapılabilir.");

        Status = RoomReservationStatus.Completed;
        CheckOutTime = DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != RoomReservationStatus.Confirmed)
            throw new DomainException("Sadece onaylanmış rezervasyonlar no-show olarak işaretlenebilir.");

        if (DateTime.UtcNow < StartTime.AddMinutes(15))
            throw new DomainException("No-show için rezervasyon başlangıcından en az 15 dakika geçmiş olmalı.");

        Status = RoomReservationStatus.NoShow;
    }

    public void SetEquipmentRequirements(
        bool requiresProjector,
        bool requiresComputer,
        bool requiresWhiteboard,
        string? specialRequirements = null)
    {
        RequiresProjector = requiresProjector;
        RequiresComputer = requiresComputer;
        RequiresWhiteboard = requiresWhiteboard;
        SpecialRequirements = specialRequirements;
    }

    public void Extend(DateTime newEndTime)
    {
        if (Status != RoomReservationStatus.InUse)
            throw new DomainException("Sadece kullanımdaki rezervasyonlar uzatılabilir.");

        if (newEndTime <= EndTime)
            throw new DomainException("Yeni bitiş zamanı mevcut bitiş zamanından sonra olmalıdır.");

        EndTime = newEndTime;
    }

    public TimeSpan GetDuration()
    {
        return EndTime - StartTime;
    }

    public bool IsActive()
    {
        return Status == RoomReservationStatus.InUse;
    }

    public bool IsUpcoming()
    {
        return Status == RoomReservationStatus.Confirmed && StartTime > DateTime.UtcNow;
    }
}