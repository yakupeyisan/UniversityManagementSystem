using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;
public class AccessGrantedEvent : BaseDomainEvent
{
    public Guid AccessPointId { get; }
    public Guid UserId { get; }
    public string AccessPointName { get; }
    public DateTime AccessTime { get; }

    public AccessGrantedEvent(Guid accessPointId, Guid userId, string accessPointName, DateTime accessTime)
    {
        AccessPointId = accessPointId;
        UserId = userId;
        AccessPointName = accessPointName;
        AccessTime = accessTime;
    }
}