using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class IncidentResolvedEvent : BaseDomainEvent
{
    public Guid IncidentId { get; }
    public Guid? SecurityZoneId { get; }
    public IncidentResolvedEvent(Guid incidentId, Guid? securityZoneId)
    {
        IncidentId = incidentId;
        SecurityZoneId = securityZoneId;
    }
}