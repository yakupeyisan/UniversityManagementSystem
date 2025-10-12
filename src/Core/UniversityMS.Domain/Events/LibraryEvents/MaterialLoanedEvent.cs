using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class MaterialLoanedEvent : BaseDomainEvent
{
    public Guid MaterialId { get; }
    public Guid BorrowerId { get; }
    public DateTime LoanDate { get; }
    public DateTime DueDate { get; }

    public MaterialLoanedEvent(Guid materialId, Guid borrowerId, DateTime loanDate, DateTime dueDate)
    {
        MaterialId = materialId;
        BorrowerId = borrowerId;
        LoanDate = loanDate;
        DueDate = dueDate;
    }
}