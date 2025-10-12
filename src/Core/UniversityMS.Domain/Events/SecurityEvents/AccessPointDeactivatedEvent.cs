using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class AccessPointDeactivatedEvent : BaseDomainEvent
{
    public Guid AccessPointId { get; }
    public string Reason { get; }

    public AccessPointDeactivatedEvent(Guid accessPointId, string reason)
    {
        AccessPointId = accessPointId;
        Reason = reason;
    }
}