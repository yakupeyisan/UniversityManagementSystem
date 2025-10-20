using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class BuildingCreatedEvent : BaseDomainEvent
{
    public Guid BuildingId { get; }
    public string Code { get; }
    public string Name { get; }
    public BuildingType Type { get; }

    public BuildingCreatedEvent(Guid buildingId, string code, string name, BuildingType type)
    {
        BuildingId = buildingId;
        Code = code;
        Name = name;
        Type = type;
    }
}

public class EquipmentUpdatedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public string? Manufacturer { get; }
    public string? Model { get; }
    public string? SerialNumber { get; }

    public EquipmentUpdatedEvent(Guid equipmentId, string? manufacturer, string? model, string? serialNumber)
    {
        EquipmentId = equipmentId;
        Manufacturer = manufacturer;
        Model = model;
        SerialNumber = serialNumber;
    }
}
public class EquipmentAssignedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public Guid AssignedToUserId { get; }
    public DateTime AssignedDate { get; }

    public EquipmentAssignedEvent(Guid equipmentId, Guid assignedToUserId, DateTime assignedDate)
    {
        EquipmentId = equipmentId;
        AssignedToUserId = assignedToUserId;
        AssignedDate = assignedDate;
    }
}

public class EquipmentReturnedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public DateTime ReturnedDate { get; }

    public EquipmentReturnedEvent(Guid equipmentId, DateTime returnedDate)
    {
        EquipmentId = equipmentId;
        ReturnedDate = returnedDate;
    }
}

public class EquipmentMaintenanceStartedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public EquipmentMaintenanceType MaintenanceType { get; }
    public DateTime StartDate { get; }

    public EquipmentMaintenanceStartedEvent(Guid equipmentId, EquipmentMaintenanceType maintenanceType, DateTime startDate)
    {
        EquipmentId = equipmentId;
        MaintenanceType = maintenanceType;
        StartDate = startDate;
    }
}

public class EquipmentDamagedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public string DamageDescription { get; }
    public DateTime DamageDate { get; }

    public EquipmentDamagedEvent(Guid equipmentId, string damageDescription, DateTime damageDate)
    {
        EquipmentId = equipmentId;
        DamageDescription = damageDescription;
        DamageDate = damageDate;
    }
}
public class EquipmentOutOfServiceEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public string Reason { get; }
    public DateTime OutOfServiceDate { get; }

    public EquipmentOutOfServiceEvent(Guid equipmentId, string reason, DateTime outOfServiceDate)
    {
        EquipmentId = equipmentId;
        Reason = reason;
        OutOfServiceDate = outOfServiceDate;
    }
}
public class EquipmentRetiredEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public string RetirementReason { get; }
    public DateTime RetirementDate { get; }

    public EquipmentRetiredEvent(Guid equipmentId, string retirementReason, DateTime retirementDate)
    {
        EquipmentId = equipmentId;
        RetirementReason = retirementReason;
        RetirementDate = retirementDate;
    }
}

public class EquipmentCalibratedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public DateTime CalibratedDate { get; }

    public EquipmentCalibratedEvent(Guid equipmentId, DateTime calibratedDate)
    {
        EquipmentId = equipmentId;
        CalibratedDate = calibratedDate;
    }
}