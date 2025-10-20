using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class VisitorCheckOutEvent : BaseDomainEvent
{
    public Guid VisitorId { get; }
    public Guid? CampusId { get; }
    public VisitorCheckOutEvent(Guid visitorId, Guid? campusId)
    {
        VisitorId = visitorId;
        CampusId = campusId;
    }
}