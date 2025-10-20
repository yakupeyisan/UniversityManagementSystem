using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

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