using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class EmergencyAlertTriggeredEvent : BaseDomainEvent
{
    public Guid AlertId { get; }
    public EmergencyAlertType AlertType { get; }
    public string Location { get; }
    public EmergencyPriority Priority { get; }
    public DateTime TriggeredAt { get; }

    public EmergencyAlertTriggeredEvent(Guid alertId, EmergencyAlertType alertType, string location, EmergencyPriority priority, DateTime triggeredAt)
    {
        AlertId = alertId;
        AlertType = alertType;
        Location = location;
        Priority = priority;
        TriggeredAt = triggeredAt;
    }
}