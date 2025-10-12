using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Park Yeri (ParkingSpace) Entity
/// </summary>
public class ParkingSpace : AuditableEntity
{
    public Guid ParkingLotId { get; private set; }
    public string SpaceNumber { get; private set; } = null!;
    public ParkingSpaceStatus Status { get; private set; }
    public bool IsDisabledSpace { get; private set; }
    public Guid? OccupiedBy { get; private set; }
    public DateTime? OccupiedAt { get; private set; }
    public Guid? ReservedBy { get; private set; }
    public DateTime? ReservationStart { get; private set; }
    public DateTime? ReservationEnd { get; private set; }
    public string? VehiclePlate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Property
    public ParkingLot ParkingLot { get; private set; } = null!;

    private ParkingSpace() { }

    private ParkingSpace(
        Guid parkingLotId,
        string spaceNumber,
        bool isDisabledSpace = false)
    {
        ParkingLotId = parkingLotId;
        SpaceNumber = spaceNumber;
        IsDisabledSpace = isDisabledSpace;
        Status = ParkingSpaceStatus.Available;
    }

    public static ParkingSpace Create(
        Guid parkingLotId,
        string spaceNumber,
        bool isDisabledSpace = false)
    {
        if (string.IsNullOrWhiteSpace(spaceNumber))
            throw new DomainException("Park yeri numarası boş olamaz.");

        return new ParkingSpace(parkingLotId, spaceNumber, isDisabledSpace);
    }

    public void Occupy(Guid? userId = null, string? vehiclePlate = null)
    {
        if (Status != ParkingSpaceStatus.Available && Status != ParkingSpaceStatus.Reserved)
            throw new DomainException("Park yeri müsait değil.");

        Status = ParkingSpaceStatus.Occupied;
        OccupiedBy = userId;
        OccupiedAt = DateTime.UtcNow;
        VehiclePlate = vehiclePlate;

        // Rezervasyon varsa temizle
        ReservedBy = null;
        ReservationStart = null;
        ReservationEnd = null;
    }

    public void Vacate()
    {
        if (Status != ParkingSpaceStatus.Occupied)
            throw new DomainException("Park yeri zaten boş.");

        Status = ParkingSpaceStatus.Available;
        OccupiedBy = null;
        OccupiedAt = null;
        VehiclePlate = null;
    }

    public void Reserve(Guid userId, DateTime? reservationEnd = null)
    {
        if (Status != ParkingSpaceStatus.Available)
            throw new DomainException("Park yeri rezerve edilemez.");

        Status = ParkingSpaceStatus.Reserved;
        ReservedBy = userId;
        ReservationStart = DateTime.UtcNow;
        ReservationEnd = reservationEnd ?? DateTime.UtcNow.AddHours(24);
    }

    public void CancelReservation()
    {
        if (Status != ParkingSpaceStatus.Reserved)
            throw new DomainException("Rezervasyon bulunmuyor.");

        Status = ParkingSpaceStatus.Available;
        ReservedBy = null;
        ReservationStart = null;
        ReservationEnd = null;
    }

    public void SetOutOfService(string reason)
    {
        Status = ParkingSpaceStatus.OutOfService;
        Notes = $"Hizmet dışı: {reason}";
    }

    public void ReturnToService()
    {
        if (Status != ParkingSpaceStatus.OutOfService)
            throw new DomainException("Park yeri zaten hizmette.");

        Status = ParkingSpaceStatus.Available;
        Notes = null;
    }

    public void CheckReservationExpiry()
    {
        if (Status == ParkingSpaceStatus.Reserved &&
            ReservationEnd.HasValue &&
            DateTime.UtcNow > ReservationEnd.Value)
        {
            CancelReservation();
        }
    }

    public TimeSpan? GetOccupancyDuration()
    {
        if (!OccupiedAt.HasValue)
            return null;

        return DateTime.UtcNow - OccupiedAt.Value;
    }

    public bool IsAvailable()
    {
        return Status == ParkingSpaceStatus.Available;
    }

    public bool IsReservationExpired()
    {
        return Status == ParkingSpaceStatus.Reserved &&
               ReservationEnd.HasValue &&
               DateTime.UtcNow > ReservationEnd.Value;
    }
}