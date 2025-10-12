using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class EmergencyAlertResolvedEvent : BaseDomainEvent
{
    public Guid AlertId { get; }
    public Guid ResolvedBy { get; }
    public DateTime ResolvedAt { get; }

    public EmergencyAlertResolvedEvent(Guid alertId, Guid resolvedBy, DateTime resolvedAt)
    {
        AlertId = alertId;
        ResolvedBy = resolvedBy;
        ResolvedAt = resolvedAt;
    }
}