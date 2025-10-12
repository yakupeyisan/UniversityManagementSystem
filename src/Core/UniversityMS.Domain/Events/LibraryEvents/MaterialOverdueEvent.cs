using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class MaterialOverdueEvent : BaseDomainEvent
{
    public Guid LoanId { get; }
    public Guid MaterialId { get; }
    public Guid BorrowerId { get; }
    public int OverdueDays { get; }

    public MaterialOverdueEvent(Guid loanId, Guid materialId, Guid borrowerId, int overdueDays)
    {
        LoanId = loanId;
        MaterialId = materialId;
        BorrowerId = borrowerId;
        OverdueDays = overdueDays;
    }
}