using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.LibraryEvents;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.LibraryAggregate;

/// <summary>
/// Rezervasyon (Reservation) Entity
/// </summary>
public class Reservation : AuditableEntity
{
    public Guid MaterialId { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime ReservationDate { get; private set; }
    public DateTime ExpiryDate { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime? NotifiedDate { get; private set; }
    public DateTime? PickupDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public Material Material { get; private set; } = null!;
    public Person User { get; private set; } = null!;

    private Reservation() { }

    private Reservation(
        Guid materialId,
        Guid userId,
        DateTime reservationDate,
        int expiryDays = 7)
    {
        MaterialId = materialId;
        UserId = userId;
        ReservationDate = reservationDate;
        ExpiryDate = reservationDate.AddDays(expiryDays);
        Status = ReservationStatus.Active;
    }

    public static Reservation Create(
        Guid materialId,
        Guid userId,
        DateTime reservationDate,
        int expiryDays = 7)
    {
        if (expiryDays <= 0)
            throw new DomainException("Son kullanma süresi pozitif olmalıdır.");

        var reservation = new Reservation(materialId, userId, reservationDate, expiryDays);
        reservation.AddDomainEvent(new MaterialReservedEvent(materialId, userId, reservationDate));

        return reservation;
    }

    public void MarkAsReady()
    {
        if (Status != ReservationStatus.Active)
            throw new DomainException("Sadece aktif rezervasyonlar hazır olarak işaretlenebilir.");

        Status = ReservationStatus.Ready;
        NotifiedDate = DateTime.UtcNow;
    }

    public void Complete(DateTime pickupDate)
    {
        if (Status != ReservationStatus.Ready)
            throw new DomainException("Sadece hazır rezervasyonlar tamamlanabilir.");

        Status = ReservationStatus.Completed;
        PickupDate = pickupDate;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == ReservationStatus.Completed)
            throw new DomainException("Tamamlanmış rezervasyon iptal edilemez.");

        Status = ReservationStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(reason))
            Notes = $"İptal sebebi: {reason}";
    }

    public void CheckExpiry()
    {
        if ((Status == ReservationStatus.Active || Status == ReservationStatus.Ready) &&
            DateTime.UtcNow > ExpiryDate)
        {
            Status = ReservationStatus.Expired;
        }
    }

    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiryDate;
    }

    public int GetDaysUntilExpiry()
    {
        if (IsExpired()) return 0;
        return (ExpiryDate - DateTime.UtcNow).Days;
    }
}