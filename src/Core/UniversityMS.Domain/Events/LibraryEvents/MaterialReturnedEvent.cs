using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class MaterialReturnedEvent : BaseDomainEvent
{
    public Guid MaterialId { get; }
    public Guid BorrowerId { get; }
    public DateTime ReturnDate { get; }

    public MaterialReturnedEvent(Guid materialId, Guid borrowerId, DateTime returnDate)
    {
        MaterialId = materialId;
        BorrowerId = borrowerId;
        ReturnDate = returnDate;
    }
}