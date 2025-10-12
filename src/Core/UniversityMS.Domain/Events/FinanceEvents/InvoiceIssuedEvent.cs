using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.FinanceEvents;

public class InvoiceIssuedEvent : BaseDomainEvent
{
    public Guid InvoiceId { get; }
    public string InvoiceNumber { get; }
    public Money TotalAmount { get; }

    public InvoiceIssuedEvent(Guid invoiceId, string invoiceNumber, Money totalAmount)
    {
        InvoiceId = invoiceId;
        InvoiceNumber = invoiceNumber;
        TotalAmount = totalAmount;
    }
}