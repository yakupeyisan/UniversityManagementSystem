using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

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