using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.FacilityEvents;

public class RoomReservedEvent : BaseDomainEvent
{
    public Guid RoomId { get; }
    public Guid ReservedBy { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }
    public string Purpose { get; }

    public RoomReservedEvent(Guid roomId, Guid reservedBy, DateTime startTime, DateTime endTime, string purpose)
    {
        RoomId = roomId;
        ReservedBy = reservedBy;
        StartTime = startTime;
        EndTime = endTime;
        Purpose = purpose;
    }
}