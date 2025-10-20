using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

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