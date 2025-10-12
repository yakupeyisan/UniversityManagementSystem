using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class BuildingStatusChangedEvent : BaseDomainEvent
{
    public Guid BuildingId { get; }
    public BuildingStatus OldStatus { get; }
    public BuildingStatus NewStatus { get; }

    public BuildingStatusChangedEvent(Guid buildingId, BuildingStatus oldStatus, BuildingStatus newStatus)
    {
        BuildingId = buildingId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}