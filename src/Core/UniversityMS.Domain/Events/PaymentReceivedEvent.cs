using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events;

public class PaymentReceivedEvent : BaseDomainEvent
{
    public Guid StudentId { get; }
    public Money Amount { get; }
    public string PaymentMethod { get; }
    public string TransactionId { get; }

    public PaymentReceivedEvent(Guid studentId, Money amount,
        string paymentMethod, string transactionId)
    {
        StudentId = studentId;
        Amount = amount;
        PaymentMethod = paymentMethod;
        TransactionId = transactionId;
    }
}