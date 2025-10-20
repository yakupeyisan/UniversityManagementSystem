using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class VisitorCheckInEvent : BaseDomainEvent
{
    public Guid VisitorId { get; }
    public Guid? CampusId { get; }
    public VisitorCheckInEvent(Guid visitorId, Guid? campusId)
    {
        VisitorId = visitorId;
        CampusId = campusId;
    }
}