using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.LibraryEvents;

public class MaterialReservedEvent : BaseDomainEvent
{
    public Guid MaterialId { get; }
    public Guid UserId { get; }
    public DateTime ReservationDate { get; }

    public MaterialReservedEvent(Guid materialId, Guid userId, DateTime reservationDate)
    {
        MaterialId = materialId;
        UserId = userId;
        ReservationDate = reservationDate;
    }
}