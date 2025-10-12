using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class RoomReservationCancelledEvent : BaseDomainEvent
{
    public Guid ReservationId { get; }
    public Guid RoomId { get; }
    public string Reason { get; }

    public RoomReservationCancelledEvent(Guid reservationId, Guid roomId, string reason)
    {
        ReservationId = reservationId;
        RoomId = roomId;
        Reason = reason;
    }
}