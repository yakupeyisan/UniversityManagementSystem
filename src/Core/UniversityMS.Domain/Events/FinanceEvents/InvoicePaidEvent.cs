using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.FinanceEvents;

public class InvoicePaidEvent : BaseDomainEvent
{
    public Guid InvoiceId { get; }
    public DateTime PaymentDate { get; }
    public Money Amount { get; }

    public InvoicePaidEvent(Guid invoiceId, DateTime paymentDate, Money amount)
    {
        InvoiceId = invoiceId;
        PaymentDate = paymentDate;
        Amount = amount;
    }
}