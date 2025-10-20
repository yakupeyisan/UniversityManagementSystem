using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class VisitorBadgeIssuedEvent : BaseDomainEvent
{
    public Guid VisitorId { get; }
    public string BadgeNumber { get; }
    public VisitorBadgeIssuedEvent(Guid visitorId, string badgeNumber)
    {
        VisitorId = visitorId;
        BadgeNumber = badgeNumber;
    }
}