using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class AccessPointActivatedEvent : BaseDomainEvent
{
    public Guid AccessPointId { get; }
    public string Name { get; }
    public string Location { get; }

    public AccessPointActivatedEvent(Guid accessPointId, string name, string location)
    {
        AccessPointId = accessPointId;
        Name = name;
        Location = location;
    }
}