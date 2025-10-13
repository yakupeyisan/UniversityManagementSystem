using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.FacilityEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Bina (Building) - Aggregate Root
/// Kampüs binalarını ve içindeki tesisleri yönetir
/// </summary>
public class Building : AuditableEntity, IAggregateRoot
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid CampusId { get; private set; }
    public BuildingType Type { get; private set; }
    public BuildingStatus Status { get; private set; }
    public Address Address { get; private set; } = null!;
    public int FloorCount { get; private set; }
    public decimal TotalArea { get; private set; } // m²
    public int? YearBuilt { get; private set; }
    public int? Capacity { get; private set; }
    public bool HasElevator { get; private set; }
    public bool HasDisabledAccess { get; private set; }
    public bool HasFireAlarm { get; private set; }
    public bool HasSprinkler { get; private set; }
    public Guid? ManagerId { get; private set; }
    public string? Description { get; private set; }
    public DateTime? LastInspectionDate { get; private set; }
    public DateTime? NextInspectionDate { get; private set; }
    public string? EmergencyExitInfo { get; private set; }

    // Navigation Property - EKLENDI
    public AcademicAggregate.Campus Campus { get; private set; } = null!;

    // Collections
    private readonly List<Room> _rooms = new();
    public IReadOnlyCollection<Room> Rooms => _rooms.AsReadOnly();

    private readonly List<Laboratory> _laboratories = new();
    public IReadOnlyCollection<Laboratory> Laboratories => _laboratories.AsReadOnly();

    private readonly List<Equipment> _equipment = new();
    public IReadOnlyCollection<Equipment> Equipment => _equipment.AsReadOnly();

    private Building() { }

    private Building(
        string code,
        string name,
        Guid campusId,
        BuildingType type,
        Address address,
        int floorCount,
        decimal totalArea,
        int? yearBuilt = null)
    {
        Code = code;
        Name = name;
        CampusId = campusId;
        Type = type;
        Address = address;
        FloorCount = floorCount;
        TotalArea = totalArea;
        YearBuilt = yearBuilt;
        Status = BuildingStatus.Active;
        HasElevator = false;
        HasDisabledAccess = false;
        HasFireAlarm = false;
        HasSprinkler = false;
    }

    public static Building Create(
        string code,
        string name,
        Guid campusId,
        BuildingType type,
        Address address,
        int floorCount,
        decimal totalArea,
        int? yearBuilt = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Bina kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Bina adı boş olamaz.");

        if (floorCount <= 0)
            throw new DomainException("Kat sayısı pozitif olmalıdır.");

        if (totalArea <= 0)
            throw new DomainException("Toplam alan pozitif olmalıdır.");

        var building = new Building(code, name, campusId, type, address, floorCount, totalArea, yearBuilt);
        building.AddDomainEvent(new BuildingCreatedEvent(building.Id, code, name, type));

        return building;
    }

    #region Building Management

    public void UpdateStatus(BuildingStatus newStatus)
    {
        var oldStatus = Status;
        Status = newStatus;

        AddDomainEvent(new BuildingStatusChangedEvent(Id, oldStatus, newStatus));
    }

    public void SetUnderConstruction()
    {
        UpdateStatus(BuildingStatus.UnderConstruction);
    }

    public void SetUnderRenovation()
    {
        UpdateStatus(BuildingStatus.UnderRenovation);
    }

    public void Activate()
    {
        UpdateStatus(BuildingStatus.Active);
    }

    public void Close(string reason)
    {
        UpdateStatus(BuildingStatus.Closed);
        Description = $"Kapatma sebebi: {reason}";
    }

    public void SetManager(Guid managerId)
    {
        ManagerId = managerId;
    }

    public void UpdateSafetyFeatures(
        bool hasElevator,
        bool hasDisabledAccess,
        bool hasFireAlarm,
        bool hasSprinkler,
        string? emergencyExitInfo = null)
    {
        HasElevator = hasElevator;
        HasDisabledAccess = hasDisabledAccess;
        HasFireAlarm = hasFireAlarm;
        HasSprinkler = hasSprinkler;
        EmergencyExitInfo = emergencyExitInfo;
    }

    public void ScheduleInspection(DateTime inspectionDate)
    {
        NextInspectionDate = inspectionDate;
    }

    public void CompleteInspection()
    {
        LastInspectionDate = DateTime.UtcNow;
        NextInspectionDate = DateTime.UtcNow.AddYears(1);
    }

    #endregion

    #region Room Management

    public void AddRoom(Room room)
    {
        if (room.BuildingId != Id)
            throw new DomainException("Oda bu binaya ait değil.");

        _rooms.Add(room);

        AddDomainEvent(new RoomAddedToBuildingEvent(Id, room.Id, room.RoomNumber, room.Type));
    }

    public void RemoveRoom(Guid roomId)
    {
        var room = _rooms.FirstOrDefault(r => r.Id == roomId);
        if (room != null)
            _rooms.Remove(room);
    }

    public Room? GetRoomByNumber(string roomNumber)
    {
        return _rooms.FirstOrDefault(r => r.RoomNumber == roomNumber);
    }

    #endregion

    #region Laboratory Management

    public void AddLaboratory(Laboratory laboratory)
    {
        if (laboratory.BuildingId != Id)
            throw new DomainException("Laboratuvar bu binaya ait değil.");

        _laboratories.Add(laboratory);
    }

    public void RemoveLaboratory(Guid labId)
    {
        var lab = _laboratories.FirstOrDefault(l => l.Id == labId);
        if (lab != null)
            _laboratories.Remove(lab);
    }

    #endregion

    #region Equipment Management

    public void AddEquipment(Equipment equipment)
    {
        if (equipment.BuildingId != Id)
            throw new DomainException("Ekipman bu binaya ait değil.");

        _equipment.Add(equipment);
    }

    public void RemoveEquipment(Guid equipmentId)
    {
        var eq = _equipment.FirstOrDefault(e => e.Id == equipmentId);
        if (eq != null)
            _equipment.Remove(eq);
    }

    #endregion

    #region Query Methods

    public int GetTotalRoomCount()
    {
        return _rooms.Count;
    }

    public int GetAvailableRoomCount()
    {
        return _rooms.Count(r => r.Status == RoomStatus.Available);
    }

    public int GetRoomCountByType(RoomType type)
    {
        return _rooms.Count(r => r.Type == type);
    }

    public int GetLaboratoryCount()
    {
        return _laboratories.Count;
    }

    public int GetEquipmentCount()
    {
        return _equipment.Count;
    }

    public bool IsOperational()
    {
        return Status == BuildingStatus.Active;
    }

    public bool NeedsInspection()
    {
        return NextInspectionDate.HasValue &&
               DateTime.UtcNow >= NextInspectionDate.Value;
    }

    public int GetBuildingAge()
    {
        if (!YearBuilt.HasValue) return 0;
        return DateTime.UtcNow.Year - YearBuilt.Value;
    }

    #endregion
}