using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class EmergencyModeDeactivatedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public DateTime DeactivationTime { get; }
    public EmergencyModeDeactivatedEvent(Guid securityZoneId, DateTime deactivationTime)
    {
        SecurityZoneId = securityZoneId;
        DeactivationTime = deactivationTime;
    }
}