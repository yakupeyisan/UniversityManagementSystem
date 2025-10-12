using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class EquipmentMaintenanceScheduledEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public MaintenanceType Type { get; }
    public DateTime ScheduledDate { get; }

    public EquipmentMaintenanceScheduledEvent(Guid equipmentId, MaintenanceType type, DateTime scheduledDate)
    {
        EquipmentId = equipmentId;
        Type = type;
        ScheduledDate = scheduledDate;
    }
}