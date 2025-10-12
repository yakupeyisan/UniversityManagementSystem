using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class ParkingSpaceVacatedEvent : BaseDomainEvent
{
    public Guid ParkingLotId { get; }
    public string SpaceNumber { get; }
    public DateTime VacatedAt { get; }

    public ParkingSpaceVacatedEvent(Guid parkingLotId, string spaceNumber, DateTime vacatedAt)
    {
        ParkingLotId = parkingLotId;
        SpaceNumber = spaceNumber;
        VacatedAt = vacatedAt;
    }
}