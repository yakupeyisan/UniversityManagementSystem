using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class VisitorCheckedInEvent : BaseDomainEvent
{
    public Guid VisitorId { get; }
    public string VisitorName { get; }
    public Guid HostId { get; }
    public DateTime CheckInTime { get; }

    public VisitorCheckedInEvent(Guid visitorId, string visitorName, Guid hostId, DateTime checkInTime)
    {
        VisitorId = visitorId;
        VisitorName = visitorName;
        HostId = hostId;
        CheckInTime = checkInTime;
    }
}