using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Yemekhane bakiyesi yüklendiğinde tetiklenen event
/// </summary>
public class CafeteriaBalanceLoadedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public Money Amount { get; }

    public CafeteriaBalanceLoadedEvent(Guid userId, Money amount)
    {
        UserId = userId;
        Amount = amount;
    }
}