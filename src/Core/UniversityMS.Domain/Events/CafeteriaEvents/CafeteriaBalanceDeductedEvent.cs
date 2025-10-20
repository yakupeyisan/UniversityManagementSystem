using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.CafeteriaEvents;

/// <summary>
/// Yemekhane bakiyesinden harcama yapıldığında tetiklenen event
/// </summary>
public class CafeteriaBalanceDeductedEvent : BaseDomainEvent
{
    public Guid UserId { get; }
    public Money Amount { get; }
    public string Description { get; }

    public CafeteriaBalanceDeductedEvent(Guid userId, Money amount, string description)
    {
        UserId = userId;
        Amount = amount;
        Description = description;
    }
}