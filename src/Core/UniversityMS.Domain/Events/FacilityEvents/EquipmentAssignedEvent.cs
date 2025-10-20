using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

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