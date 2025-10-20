using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class IncidentReportedEvent : BaseDomainEvent
{
    public Guid IncidentId { get; }
    public Guid? SecurityZoneId { get; }
    public IncidentSeverity Severity { get; }
    public IncidentReportedEvent(Guid incidentId, Guid? securityZoneId, IncidentSeverity severity)
    {
        IncidentId = incidentId;
        SecurityZoneId = securityZoneId;
        Severity = severity;
    }
}