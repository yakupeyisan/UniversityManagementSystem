using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.FacilityEvents;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.FacilityAggregate;

/// <summary>
/// Laboratuvar (Laboratory) Entity
/// </summary>
public class Laboratory : AuditableEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public Guid BuildingId { get; private set; }
    public Guid? RoomId { get; private set; }
    public LaboratoryType Type { get; private set; }
    public SafetyLevel SafetyLevel { get; private set; }
    public RoomStatus Status { get; private set; }
    public int Capacity { get; private set; }
    public Guid? SupervisorId { get; private set; }
    public bool RequiresSafetyTraining { get; private set; }
    public bool HasEmergencyShower { get; private set; }
    public bool HasEyeWash { get; private set; }
    public bool HasFireExtinguisher { get; private set; }
    public bool HasVentilation { get; private set; }
    public string? SafetyProtocols { get; private set; }
    public string? AvailableEquipment { get; private set; }
    public string? Description { get; private set; }
    public DateTime? LastSafetyInspection { get; private set; }
    public DateTime? NextSafetyInspection { get; private set; }

    // Navigation Properties
    public Building Building { get; private set; } = null!;
    public Room? Room { get; private set; }

    // Collections
    private readonly List<Equipment> _equipment = new();
    public IReadOnlyCollection<Equipment> Equipment => _equipment.AsReadOnly();

    private Laboratory() { }

    private Laboratory(
        string code,
        string name,
        Guid buildingId,
        LaboratoryType type,
        SafetyLevel safetyLevel,
        int capacity,
        Guid? roomId = null)
    {
        Code = code;
        Name = name;
        BuildingId = buildingId;
        RoomId = roomId;
        Type = type;
        SafetyLevel = safetyLevel;
        Capacity = capacity;
        Status = RoomStatus.Available;
        RequiresSafetyTraining = safetyLevel >= SafetyLevel.Level2;
        HasEmergencyShower = false;
        HasEyeWash = false;
        HasFireExtinguisher = false;
        HasVentilation = false;
    }

    public static Laboratory Create(
        string code,
        string name,
        Guid buildingId,
        LaboratoryType type,
        SafetyLevel safetyLevel,
        int capacity,
        Guid? roomId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Laboratuvar kodu boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Laboratuvar adı boş olamaz.");

        if (capacity <= 0)
            throw new DomainException("Kapasite pozitif olmalıdır.");

        var lab = new Laboratory(code, name, buildingId, type, safetyLevel, capacity, roomId);
        lab.AddDomainEvent(new LaboratoryCreatedEvent(lab.Id, code, type, safetyLevel));

        return lab;
    }

    public void UpdateSafetyFeatures(
        bool hasEmergencyShower,
        bool hasEyeWash,
        bool hasFireExtinguisher,
        bool hasVentilation,
        string? safetyProtocols = null)
    {
        HasEmergencyShower = hasEmergencyShower;
        HasEyeWash = hasEyeWash;
        HasFireExtinguisher = hasFireExtinguisher;
        HasVentilation = hasVentilation;
        SafetyProtocols = safetyProtocols;
    }

    public void SetSupervisor(Guid supervisorId)
    {
        SupervisorId = supervisorId;
    }

    public void ScheduleSafetyInspection(DateTime inspectionDate)
    {
        NextSafetyInspection = inspectionDate;
    }

    public void CompleteSafetyInspection()
    {
        LastSafetyInspection = DateTime.UtcNow;

        // Güvenlik seviyesine göre sonraki denetim tarihi
        var monthsUntilNext = SafetyLevel switch
        {
            SafetyLevel.Level1 => 12,
            SafetyLevel.Level2 => 6,
            SafetyLevel.Level3 => 3,
            SafetyLevel.Level4 => 1,
            _ => 6
        };

        NextSafetyInspection = DateTime.UtcNow.AddMonths(monthsUntilNext);
    }

    public void AddEquipment(Equipment equipment)
    {
        if (equipment.LaboratoryId != Id)
            throw new DomainException("Ekipman bu laboratuvara ait değil.");

        _equipment.Add(equipment);
    }

    public void MakeAvailable()
    {
        Status = RoomStatus.Available;
    }

    public void SetUnderMaintenance()
    {
        Status = RoomStatus.UnderMaintenance;
    }

    public void Lock()
    {
        Status = RoomStatus.Locked;
    }

    public bool NeedsSafetyInspection()
    {
        return NextSafetyInspection.HasValue &&
               DateTime.UtcNow >= NextSafetyInspection.Value;
    }

    public bool MeetsSafetyRequirements()
    {
        return SafetyLevel switch
        {
            SafetyLevel.Level1 => HasFireExtinguisher,
            SafetyLevel.Level2 => HasFireExtinguisher && HasEyeWash,
            SafetyLevel.Level3 => HasFireExtinguisher && HasEyeWash && HasEmergencyShower && HasVentilation,
            SafetyLevel.Level4 => HasFireExtinguisher && HasEyeWash && HasEmergencyShower && HasVentilation,
            _ => false
        };
    }
}