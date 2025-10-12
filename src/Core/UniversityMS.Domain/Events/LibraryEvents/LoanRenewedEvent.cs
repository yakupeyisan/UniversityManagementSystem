using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class LoanRenewedEvent : BaseDomainEvent
{
    public Guid LoanId { get; }
    public Guid MaterialId { get; }
    public DateTime NewDueDate { get; }

    public LoanRenewedEvent(Guid loanId, Guid materialId, DateTime newDueDate)
    {
        LoanId = loanId;
        MaterialId = materialId;
        NewDueDate = newDueDate;
    }
}