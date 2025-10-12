using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.FinanceEvents;

public class TransactionCreatedEvent : BaseDomainEvent
{
    public Guid TransactionId { get; }
    public string TransactionNumber { get; }
    public TransactionType Type { get; }
    public Money Amount { get; }

    public TransactionCreatedEvent(Guid transactionId, string transactionNumber, TransactionType type, Money amount)
    {
        TransactionId = transactionId;
        TransactionNumber = transactionNumber;
        Type = type;
        Amount = amount;
    }
}