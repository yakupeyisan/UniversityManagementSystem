using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Menü oluşturulduğunda tetiklenen event
/// </summary>
public class MenuCreatedEvent : BaseDomainEvent
{
    public Guid MenuId { get; }
    public string Name { get; }
    public MenuType Type { get; }

    public MenuCreatedEvent(Guid menuId, string name, MenuType type)
    {
        MenuId = menuId;
        Name = name;
        Type = type;
    }
}

/// <summary>
/// CafeteriaAccount'a AccessTemplate atandı
/// </summary>
public class CafeteriaTemplateAssignedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public Guid TemplateId { get; }

    public CafeteriaTemplateAssignedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        Guid templateId)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        TemplateId = templateId;
    }
}

/// <summary>
/// CafeteriaAccount'tan AccessTemplate kaldırıldı
/// </summary>
public class CafeteriaTemplateRemovedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public Guid TemplateId { get; }

    public CafeteriaTemplateRemovedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        Guid templateId)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        TemplateId = templateId;
    }
}

/// <summary>
/// CafeteriaAccount'a ekstra SecurityZone erişimi verildi
/// </summary>
public class CafeteriaAdditionalAccessGrantedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public Guid SecurityZoneId { get; }
    public string Reason { get; }

    public CafeteriaAdditionalAccessGrantedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        Guid securityZoneId,
        string reason)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        SecurityZoneId = securityZoneId;
        Reason = reason;
    }
}

/// <summary>
/// CafeteriaAccount'tan ekstra SecurityZone erişimi kaldırıldı
/// </summary>
public class CafeteriaAdditionalAccessRevokedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public Guid SecurityZoneId { get; }

    public CafeteriaAdditionalAccessRevokedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        Guid securityZoneId)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        SecurityZoneId = securityZoneId;
    }
}

/// <summary>
/// CafeteriaAccount'ın SecurityZone erişimi engellendi
/// </summary>
public class CafeteriaAccessExcludedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public Guid SecurityZoneId { get; }
    public string Reason { get; }

    public CafeteriaAccessExcludedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        Guid securityZoneId,
        string reason)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        SecurityZoneId = securityZoneId;
        Reason = reason;
    }
}

/// <summary>
/// CafeteriaAccount'ın engellenen SecurityZone erişimi açıldı
/// </summary>
public class CafeteriaAccessIncludedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public Guid SecurityZoneId { get; }

    public CafeteriaAccessIncludedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        Guid securityZoneId)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        SecurityZoneId = securityZoneId;
    }
}

public class CafeteriaAccountCreatedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public string CardNumber { get; }
    public decimal InitialBalance { get; }

    public CafeteriaAccountCreatedEvent(
        Guid userId,
        string cardNumber,
        decimal initialBalance)
    {
        UserId = userId;
        CardNumber = cardNumber;
        InitialBalance = initialBalance;
    }
}

public class CafeteriaBalanceLoadedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public decimal Amount { get; }
    public string Reason { get; }

    public CafeteriaBalanceLoadedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        decimal amount,
        string reason)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        Amount = amount;
        Reason = reason;
    }
}

public class CafeteriaTransactionCompletedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public decimal Amount { get; }
    public string Description { get; }

    public CafeteriaTransactionCompletedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        decimal amount,
        string description)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        Amount = amount;
        Description = description;
    }
}

public class CafeteriaAccountLockedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }
    public string Reason { get; }

    public CafeteriaAccountLockedEvent(
        Guid cafeteriaAccountId,
        Guid userId,
        string reason)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
        Reason = reason;
    }
}

public class CafeteriaAccountUnlockedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }

    public CafeteriaAccountUnlockedEvent(Guid cafeteriaAccountId, Guid userId)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
    }
}

public class CafeteriaCardRenewedEvent : BaseDomainEvent
{
    public Guid CafeteriaAccountId { get; }
    public Guid UserId { get; }

    public CafeteriaCardRenewedEvent(Guid cafeteriaAccountId, Guid userId)
    {
        CafeteriaAccountId = cafeteriaAccountId;
        UserId = userId;
    }
}