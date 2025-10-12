using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class FinePaidEvent : BaseDomainEvent
{
    public Guid FineId { get; }
    public Guid UserId { get; }
    public Money Amount { get; }
    public DateTime PaymentDate { get; }

    public FinePaidEvent(Guid fineId, Guid userId, Money amount, DateTime paymentDate)
    {
        FineId = fineId;
        UserId = userId;
        Amount = amount;
        PaymentDate = paymentDate;
    }
}