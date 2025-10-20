using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// IT talebi atandığında tetiklenen event
/// </summary>
public class ITTicketAssignedEvent : BaseDomainEvent
{
    public Guid TicketId { get; }
    public string TicketNumber { get; }
    public Guid AssignedTo { get; }
    public DateTime AssignedDate { get; }

    public ITTicketAssignedEvent(Guid ticketId, string ticketNumber, Guid assignedTo, DateTime assignedDate)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        AssignedTo = assignedTo;
        AssignedDate = assignedDate;
    }
}