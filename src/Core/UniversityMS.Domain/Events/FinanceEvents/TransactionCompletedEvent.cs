using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FinanceEvents;

public class TransactionCompletedEvent : BaseDomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }

    public TransactionCompletedEvent(Guid transactionId, string referenceNumber)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
    }
}