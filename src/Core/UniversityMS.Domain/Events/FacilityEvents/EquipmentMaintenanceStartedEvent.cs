using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

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