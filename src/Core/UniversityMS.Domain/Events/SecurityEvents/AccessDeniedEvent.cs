using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class AccessDeniedEvent : BaseDomainEvent
{
    public Guid AccessPointId { get; }
    public Guid? UserId { get; }
    public string AccessPointName { get; }
    public AccessDenialReason Reason { get; }
    public DateTime AttemptTime { get; }

    public AccessDeniedEvent(Guid accessPointId, Guid? userId, string accessPointName, AccessDenialReason reason, DateTime attemptTime)
    {
        AccessPointId = accessPointId;
        UserId = userId;
        AccessPointName = accessPointName;
        Reason = reason;
        AttemptTime = attemptTime;
    }
}