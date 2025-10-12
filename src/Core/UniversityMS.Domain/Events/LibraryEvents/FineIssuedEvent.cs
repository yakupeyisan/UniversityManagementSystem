using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class FineIssuedEvent : BaseDomainEvent
{
    public Guid FineId { get; }
    public Guid UserId { get; }
    public Money Amount { get; }
    public string Reason { get; }

    public FineIssuedEvent(Guid fineId, Guid userId, Money amount, string reason)
    {
        FineId = fineId;
        UserId = userId;
        Amount = amount;
        Reason = reason;
    }
}