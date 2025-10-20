using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.SecurityEvents;

public class SecurityRuleCreatedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public Guid RuleId { get; }
    public SecurityRuleCreatedEvent(Guid securityZoneId, Guid ruleId)
    {
        SecurityZoneId = securityZoneId;
        RuleId = ruleId;
    }
}