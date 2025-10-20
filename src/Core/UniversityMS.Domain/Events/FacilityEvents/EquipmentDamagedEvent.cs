using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

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