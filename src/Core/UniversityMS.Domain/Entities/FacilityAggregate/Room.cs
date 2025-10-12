using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Oda/Sınıf (Room) Entity
/// </summary>
public class Room : AuditableEntity
{
    public Guid BuildingId { get; private set; }
    public string RoomNumber { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public int Floor { get; private set; }
    public RoomType Type { get; private set; }
    public RoomStatus Status { get; private set; }
    public int Capacity { get; private set; }
    public decimal Area { get; private set; } // m²
    public bool HasProjector { get; private set; }
    public bool HasWhiteboard { get; private set; }
    public bool HasComputer { get; private set; }
    public bool HasAirConditioning { get; private set; }
    public bool IsAccessible { get; private set; } // Engelli erişimi
    public string? Description { get; private set; }
    public string? Equipment { get; private set; }

    // Navigation Properties
    public Building Building { get; private set; } = null!;

    // Collections
    private readonly List<RoomReservation> _reservations = new();
    public IReadOnlyCollection<RoomReservation> Reservations => _reservations.AsReadOnly();

    private Room() { }

    private Room(
        Guid buildingId,
        string roomNumber,
        string name,
        int floor,
        RoomType type,
        int capacity,
        decimal area)
    {
        BuildingId = buildingId;
        RoomNumber = roomNumber;
        Name = name;
        Floor = floor;
        Type = type;
        Capacity = capacity;
        Area = area;
        Status = RoomStatus.Available;
        HasProjector = false;
        HasWhiteboard = false;
        HasComputer = false;
        HasAirConditioning = false;
        IsAccessible = false;
    }

    public static Room Create(
        Guid buildingId,
        string roomNumber,
        string name,
        int floor,
        RoomType type,
        int capacity,
        decimal area)
    {
        if (string.IsNullOrWhiteSpace(roomNumber))
            throw new DomainException("Oda numarası boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Oda adı boş olamaz.");

        if (capacity <= 0)
            throw new DomainException("Kapasite pozitif olmalıdır.");

        if (area <= 0)
            throw new DomainException("Alan pozitif olmalıdır.");

        return new Room(buildingId, roomNumber, name, floor, type, capacity, area);
    }

    public void UpdateEquipment(
        bool hasProjector,
        bool hasWhiteboard,
        bool hasComputer,
        bool hasAirConditioning,
        bool isAccessible,
        string? additionalEquipment = null)
    {
        HasProjector = hasProjector;
        HasWhiteboard = hasWhiteboard;
        HasComputer = hasComputer;
        HasAirConditioning = hasAirConditioning;
        IsAccessible = isAccessible;
        Equipment = additionalEquipment;
    }

    public void MakeAvailable()
    {
        if (Status == RoomStatus.UnderMaintenance || Status == RoomStatus.OutOfService)
            throw new DomainException("Bakımdaki veya hizmet dışı odalar müsait yapılamaz.");

        Status = RoomStatus.Available;
    }

    public void Lock()
    {
        Status = RoomStatus.Locked;
    }

    public void Unlock()
    {
        Status = RoomStatus.Available;
    }

    public void SetUnderMaintenance()
    {
        Status = RoomStatus.UnderMaintenance;
    }

    public void SetOutOfService(string reason)
    {
        Status = RoomStatus.OutOfService;
        Description = $"Hizmet dışı: {reason}";
    }

    public void AddReservation(RoomReservation reservation)
    {
        if (reservation.RoomId != Id)
            throw new DomainException("Rezervasyon bu odaya ait değil.");

        // Çakışma kontrolü
        var hasConflict = _reservations.Any(r =>
            r.Status == RoomReservationStatus.Confirmed &&
            ((reservation.StartTime >= r.StartTime && reservation.StartTime < r.EndTime) ||
             (reservation.EndTime > r.StartTime && reservation.EndTime <= r.EndTime) ||
             (reservation.StartTime <= r.StartTime && reservation.EndTime >= r.EndTime)));

        if (hasConflict)
            throw new DomainException("Bu zaman diliminde zaten bir rezervasyon var.");

        _reservations.Add(reservation);
    }

    public bool IsAvailableAt(DateTime startTime, DateTime endTime)
    {
        if (Status != RoomStatus.Available)
            return false;

        return !_reservations.Any(r =>
            r.Status == RoomReservationStatus.Confirmed &&
            ((startTime >= r.StartTime && startTime < r.EndTime) ||
             (endTime > r.StartTime && endTime <= r.EndTime) ||
             (startTime <= r.StartTime && endTime >= r.EndTime)));
    }

    public int GetTodayReservationCount()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        return _reservations.Count(r =>
            DateOnly.FromDateTime(r.StartTime) == today &&
            r.Status == RoomReservationStatus.Confirmed);
    }
}