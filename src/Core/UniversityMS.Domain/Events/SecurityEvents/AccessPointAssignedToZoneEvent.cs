using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class AccessPointAssignedToZoneEvent : BaseDomainEvent
{
    public Guid AccessPointId { get; }
    public Guid SecurityZoneId { get; }
    public AccessPointAssignedToZoneEvent(Guid accessPointId, Guid securityZoneId)
    {
        AccessPointId = accessPointId;
        SecurityZoneId = securityZoneId;
    }
}