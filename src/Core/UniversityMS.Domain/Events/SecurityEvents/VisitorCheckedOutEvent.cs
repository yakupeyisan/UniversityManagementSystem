using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class VisitorCheckedOutEvent : BaseDomainEvent
{
    public Guid VisitorId { get; }
    public string VisitorName { get; }
    public DateTime CheckOutTime { get; }

    public VisitorCheckedOutEvent(Guid visitorId, string visitorName, DateTime checkOutTime)
    {
        VisitorId = visitorId;
        VisitorName = visitorName;
        CheckOutTime = checkOutTime;
    }
}