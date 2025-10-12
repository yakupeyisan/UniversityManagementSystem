using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class ParkingSpaceOccupiedEvent : BaseDomainEvent
{
    public Guid ParkingLotId { get; }
    public string SpaceNumber { get; }
    public Guid? UserId { get; }
    public DateTime OccupiedAt { get; }

    public ParkingSpaceOccupiedEvent(Guid parkingLotId, string spaceNumber, Guid? userId, DateTime occupiedAt)
    {
        ParkingLotId = parkingLotId;
        SpaceNumber = spaceNumber;
        UserId = userId;
        OccupiedAt = occupiedAt;
    }
}