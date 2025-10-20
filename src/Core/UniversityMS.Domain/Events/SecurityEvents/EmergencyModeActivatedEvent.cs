using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class EmergencyModeActivatedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public DateTime ActivationTime { get; }
    public EmergencyModeActivatedEvent(Guid securityZoneId, DateTime activationTime)
    {
        SecurityZoneId = securityZoneId;
        ActivationTime = activationTime;
    }
}