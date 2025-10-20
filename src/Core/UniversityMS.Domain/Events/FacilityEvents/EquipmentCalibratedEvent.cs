using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

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