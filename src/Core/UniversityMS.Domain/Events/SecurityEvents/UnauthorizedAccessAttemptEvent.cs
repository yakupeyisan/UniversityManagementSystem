using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class UnauthorizedAccessAttemptEvent : BaseDomainEvent
{
    public Guid AccessPointId { get; }
    public string? CredentialUsed { get; }
    public string Location { get; }
    public DateTime AttemptTime { get; }

    public UnauthorizedAccessAttemptEvent(Guid accessPointId, string? credentialUsed, string location, DateTime attemptTime)
    {
        AccessPointId = accessPointId;
        CredentialUsed = credentialUsed;
        Location = location;
        AttemptTime = attemptTime;
    }
}