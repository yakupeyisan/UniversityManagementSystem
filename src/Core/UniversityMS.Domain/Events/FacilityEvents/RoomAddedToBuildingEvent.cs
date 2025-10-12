using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class RoomAddedToBuildingEvent : BaseDomainEvent
{
    public Guid BuildingId { get; }
    public Guid RoomId { get; }
    public string RoomNumber { get; }
    public RoomType Type { get; }

    public RoomAddedToBuildingEvent(Guid buildingId, Guid roomId, string roomNumber, RoomType type)
    {
        BuildingId = buildingId;
        RoomId = roomId;
        RoomNumber = roomNumber;
        Type = type;
    }
}