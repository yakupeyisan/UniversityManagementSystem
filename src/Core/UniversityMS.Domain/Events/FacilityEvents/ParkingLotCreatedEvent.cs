using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class ParkingLotCreatedEvent : BaseDomainEvent
{
    public Guid ParkingLotId { get; }
    public string Code { get; }
    public ParkingLotType Type { get; }
    public int TotalSpaces { get; }

    public ParkingLotCreatedEvent(Guid parkingLotId, string code, ParkingLotType type, int totalSpaces)
    {
        ParkingLotId = parkingLotId;
        Code = code;
        Type = type;
        TotalSpaces = totalSpaces;
    }
}