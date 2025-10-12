using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class EquipmentAddedEvent : BaseDomainEvent
{
    public Guid EquipmentId { get; }
    public string Code { get; }
    public EquipmentType Type { get; }
    public Guid? RoomId { get; }

    public EquipmentAddedEvent(Guid equipmentId, string code, EquipmentType type, Guid? roomId)
    {
        EquipmentId = equipmentId;
        Code = code;
        Type = type;
        RoomId = roomId;
    }
}