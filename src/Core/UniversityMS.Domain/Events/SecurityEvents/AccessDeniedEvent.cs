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

/// <summary>
/// AccessTemplate oluşturuldu
/// </summary>
public class AccessTemplateCreatedEvent : BaseDomainEvent
{
    public Guid TemplateId { get; }
    public string TemplateName { get; }
    public AccessTemplateType TemplateType { get; }

    public AccessTemplateCreatedEvent(
        Guid templateId,
        string templateName,
        AccessTemplateType templateType)
    {
        TemplateId = templateId;
        TemplateName = templateName;
        TemplateType = templateType;
    }
}

/// <summary>
/// AccessTemplate deaktif edildi
/// </summary>
public class AccessTemplateDeactivatedEvent : BaseDomainEvent
{
    public Guid TemplateId { get; }
    public string TemplateName { get; }

    public AccessTemplateDeactivatedEvent(Guid templateId, string templateName)
    {
        TemplateId = templateId;
        TemplateName = templateName;
    }
}

/// <summary>
/// AccessTemplate aktif edildi
/// </summary>
public class AccessTemplateActivatedEvent : BaseDomainEvent
{
    public Guid TemplateId { get; }
    public string TemplateName { get; }

    public AccessTemplateActivatedEvent(Guid templateId, string templateName)
    {
        TemplateId = templateId;
        TemplateName = templateName;
    }
}

/// <summary>
/// AccessTemplate'e SecurityZone izni eklendi
/// </summary>
public class AccessPermissionAddedToTemplateEvent : BaseDomainEvent
{
    public Guid TemplateId { get; }
    public Guid SecurityZoneId { get; }
    public AccessLevel RequiredAccessLevel { get; }

    public AccessPermissionAddedToTemplateEvent(
        Guid templateId,
        Guid securityZoneId,
        AccessLevel requiredAccessLevel)
    {
        TemplateId = templateId;
        SecurityZoneId = securityZoneId;
        RequiredAccessLevel = requiredAccessLevel;
    }
}

/// <summary>
/// AccessTemplate'ten SecurityZone izni kaldırıldı
/// </summary>
public class AccessPermissionRemovedFromTemplateEvent : BaseDomainEvent
{
    public Guid TemplateId { get; }
    public Guid SecurityZoneId { get; }

    public AccessPermissionRemovedFromTemplateEvent(Guid templateId, Guid securityZoneId)
    {
        TemplateId = templateId;
        SecurityZoneId = securityZoneId;
    }
}


public class SecurityZoneCreatedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public string Code { get; }
    public string Name { get; }

    public SecurityZoneCreatedEvent(Guid securityZoneId, string code, string name)
    {
        SecurityZoneId = securityZoneId;
        Code = code;
        Name = name;
    }
}

public class AllowedAccessLevelAddedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public AccessLevel AccessLevel { get; }

    public AllowedAccessLevelAddedEvent(Guid securityZoneId, AccessLevel accessLevel)
    {
        SecurityZoneId = securityZoneId;
        AccessLevel = accessLevel;
    }
}

public class AllowedAccessLevelRemovedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public AccessLevel AccessLevel { get; }

    public AllowedAccessLevelRemovedEvent(Guid securityZoneId, AccessLevel accessLevel)
    {
        SecurityZoneId = securityZoneId;
        AccessLevel = accessLevel;
    }
}

public class TimeRestrictionAddedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public Guid RestrictionId { get; }
    public string RestrictionName { get; }

    public TimeRestrictionAddedEvent(
        Guid securityZoneId,
        Guid restrictionId,
        string restrictionName)
    {
        SecurityZoneId = securityZoneId;
        RestrictionId = restrictionId;
        RestrictionName = restrictionName;
    }
}

public class TimeRestrictionRemovedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public Guid RestrictionId { get; }

    public TimeRestrictionRemovedEvent(Guid securityZoneId, Guid restrictionId)
    {
        SecurityZoneId = securityZoneId;
        RestrictionId = restrictionId;
    }
}

public class SecurityRuleAddedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public Guid RuleId { get; }

    public SecurityRuleAddedEvent(Guid securityZoneId, Guid ruleId)
    {
        SecurityZoneId = securityZoneId;
        RuleId = ruleId;
    }
}

public class SecurityRuleRemovedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public Guid RuleId { get; }

    public SecurityRuleRemovedEvent(Guid securityZoneId, Guid ruleId)
    {
        SecurityZoneId = securityZoneId;
        RuleId = ruleId;
    }
}

public class EmergencyModeActivatedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public string ZoneName { get; }
    public string Reason { get; }

    public EmergencyModeActivatedEvent(Guid securityZoneId, string zoneName, string reason)
    {
        SecurityZoneId = securityZoneId;
        ZoneName = zoneName;
        Reason = reason;
    }
}

public class EmergencyModeDeactivatedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public string ZoneName { get; }

    public EmergencyModeDeactivatedEvent(Guid securityZoneId, string zoneName)
    {
        SecurityZoneId = securityZoneId;
        ZoneName = zoneName;
    }
}

public class SecurityZoneStatusChangedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public SecurityZoneStatus NewStatus { get; }

    public SecurityZoneStatusChangedEvent(Guid securityZoneId, SecurityZoneStatus newStatus)
    {
        SecurityZoneId = securityZoneId;
        NewStatus = newStatus;
    }
}

public class SecurityAuditPerformedEvent : BaseDomainEvent
{
    public Guid SecurityZoneId { get; }
    public string ZoneCode { get; }

    public SecurityAuditPerformedEvent(Guid securityZoneId, string zoneCode)
    {
        SecurityZoneId = securityZoneId;
        ZoneCode = zoneCode;
    }
}