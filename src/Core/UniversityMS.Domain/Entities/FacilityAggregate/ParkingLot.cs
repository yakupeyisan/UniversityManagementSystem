using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.FacilityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Otopark (ParkingLot) Entity
/// </summary>
public class ParkingLot : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid CampusId { get; private set; }
    public Guid? BuildingId { get; private set; }
    public ParkingLotType Type { get; private set; }
    public ParkingLotStatus Status { get; private set; }
    public Address Location { get; private set; } = null!;
    public int TotalSpaces { get; private set; }
    public int AvailableSpaces { get; private set; }
    public int ReservedSpaces { get; private set; }
    public int DisabledSpaces { get; private set; }
    public bool HasSecurity { get; private set; }
    public bool HasCCTV { get; private set; }
    public bool HasLighting { get; private set; }
    public bool IsCovered { get; private set; }
    public Money? HourlyRate { get; private set; }
    public Money? DailyRate { get; private set; }
    public Money? MonthlyRate { get; private set; }
    public TimeOnly? OpeningTime { get; private set; }
    public TimeOnly? ClosingTime { get; private set; }
    public string? Description { get; private set; }
    public Guid? ManagerId { get; private set; }

    // Collections
    private readonly List<ParkingSpace> _parkingSpaces = new();
    public IReadOnlyCollection<ParkingSpace> ParkingSpaces => _parkingSpaces.AsReadOnly();

    private ParkingLot() { }

    private ParkingLot(
        string code,
        string name,
        Guid campusId,
        ParkingLotType type,
        Address location,
        int totalSpaces,
        Guid? buildingId = null)
    {
        Code = code;
        Name = name;
        CampusId = campusId;
        BuildingId = buildingId;
        Type = type;
        Location = location;
        TotalSpaces = totalSpaces;
        AvailableSpaces = totalSpaces;
        ReservedSpaces = 0;
        DisabledSpaces = 0;
        Status = ParkingLotStatus.Open;
        HasSecurity = false;
        HasCCTV = false;
        HasLighting = false;
        IsCovered = type == ParkingLotType.MultiLevel || type == ParkingLotType.Underground;
    }

    public static ParkingLot Create(
        string code,
        string name,
        Guid campusId,
        ParkingLotType type,
        Address location,
        int totalSpaces,
        Guid? buildingId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Otopark kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Otopark adı boş olamaz.");

        if (totalSpaces <= 0)
            throw new DomainException("Toplam park yeri sayısı pozitif olmalıdır.");

        var parkingLot = new ParkingLot(code, name, campusId, type, location, totalSpaces, buildingId);
        parkingLot.AddDomainEvent(new ParkingLotCreatedEvent(parkingLot.Id, code, type, totalSpaces));

        return parkingLot;
    }

    public void InitializeParkingSpaces(int regularSpaces, int disabledSpaces)
    {
        if (regularSpaces + disabledSpaces != TotalSpaces)
            throw new DomainException("Park yeri sayıları toplamı total ile eşleşmiyor.");

        DisabledSpaces = disabledSpaces;

        // Normal park yerleri
        for (int i = 1; i <= regularSpaces; i++)
        {
            var space = ParkingSpace.Create(Id, $"{Code}-{i:D3}", false);
            _parkingSpaces.Add(space);
        }

        // Engelli park yerleri
        for (int i = 1; i <= disabledSpaces; i++)
        {
            var space = ParkingSpace.Create(Id, $"{Code}-D{i:D2}", true);
            _parkingSpaces.Add(space);
        }
    }

    public void UpdateFeatures(
        bool hasSecurity,
        bool hasCCTV,
        bool hasLighting)
    {
        HasSecurity = hasSecurity;
        HasCCTV = hasCCTV;
        HasLighting = hasLighting;
    }

    public void SetPricing(
        Money? hourlyRate = null,
        Money? dailyRate = null,
        Money? monthlyRate = null)
    {
        HourlyRate = hourlyRate;
        DailyRate = dailyRate;
        MonthlyRate = monthlyRate;
    }

    public void SetOperatingHours(TimeOnly openingTime, TimeOnly closingTime)
    {
        if (closingTime <= openingTime)
            throw new DomainException("Kapanış saati açılış saatinden sonra olmalıdır.");

        OpeningTime = openingTime;
        ClosingTime = closingTime;
    }

    public void Open()
    {
        Status = ParkingLotStatus.Open;
    }

    public void Close()
    {
        Status = ParkingLotStatus.Closed;
    }

    public void SetFull()
    {
        Status = ParkingLotStatus.Full;
    }

    public void SetUnderMaintenance()
    {
        Status = ParkingLotStatus.Maintenance;
    }

    public void OccupySpace(string spaceNumber, Guid? userId = null)
    {
        var space = _parkingSpaces.FirstOrDefault(s => s.SpaceNumber == spaceNumber);
        if (space == null)
            throw new DomainException("Park yeri bulunamadı.");

        space.Occupy(userId);
        AvailableSpaces--;

        if (AvailableSpaces == 0)
            Status = ParkingLotStatus.Full;

        AddDomainEvent(new ParkingSpaceOccupiedEvent(Id, spaceNumber, userId, DateTime.UtcNow));
    }

    public void VacateSpace(string spaceNumber)
    {
        var space = _parkingSpaces.FirstOrDefault(s => s.SpaceNumber == spaceNumber);
        if (space == null)
            throw new DomainException("Park yeri bulunamadı.");

        space.Vacate();
        AvailableSpaces++;

        if (Status == ParkingLotStatus.Full)
            Status = ParkingLotStatus.Open;

        AddDomainEvent(new ParkingSpaceVacatedEvent(Id, spaceNumber, DateTime.UtcNow));
    }

    public void ReserveSpace(string spaceNumber, Guid userId, DateTime? reservationEnd = null)
    {
        var space = _parkingSpaces.FirstOrDefault(s => s.SpaceNumber == spaceNumber);
        if (space == null)
            throw new DomainException("Park yeri bulunamadı.");

        space.Reserve(userId, reservationEnd);
        ReservedSpaces++;
        AvailableSpaces--;
    }

    public void CancelReservation(string spaceNumber)
    {
        var space = _parkingSpaces.FirstOrDefault(s => s.SpaceNumber == spaceNumber);
        if (space == null)
            throw new DomainException("Park yeri bulunamadı.");

        space.CancelReservation();
        ReservedSpaces--;
        AvailableSpaces++;
    }

    public ParkingSpace? FindAvailableSpace(bool needsDisabledSpace = false)
    {
        return _parkingSpaces
            .Where(s => s.Status == ParkingSpaceStatus.Available &&
                        s.IsDisabledSpace == needsDisabledSpace)
            .OrderBy(s => s.SpaceNumber)
            .FirstOrDefault();
    }

    public decimal GetOccupancyRate()
    {
        if (TotalSpaces == 0) return 0;
        return ((decimal)(TotalSpaces - AvailableSpaces) / TotalSpaces) * 100;
    }

    public bool IsFull()
    {
        return AvailableSpaces == 0;
    }

    public bool IsOperational()
    {
        return Status == ParkingLotStatus.Open || Status == ParkingLotStatus.Full;
    }

    public bool IsOpen24Hours()
    {
        return !OpeningTime.HasValue && !ClosingTime.HasValue;
    }
}