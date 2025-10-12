using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class EquipmentMaintenanceCompletedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public DateTime CompletedDate { get; }
    public Guid CompletedBy { get; }

    public EquipmentMaintenanceCompletedEvent(Guid equipmentId, DateTime completedDate, Guid completedBy)
    {
        EquipmentId = equipmentId;
        CompletedDate = completedDate;
        CompletedBy = completedBy;
    }
}