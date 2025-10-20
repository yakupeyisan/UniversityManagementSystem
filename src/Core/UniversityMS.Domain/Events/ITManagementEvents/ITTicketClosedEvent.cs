using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// IT talebi kapatıldığında tetiklenen event
/// </summary>
public class ITTicketClosedEvent : BaseDomainEvent
{
    public Guid TicketId { get; }
    public string TicketNumber { get; }
    public DateTime ClosedDate { get; }

    public ITTicketClosedEvent(Guid ticketId, string ticketNumber, DateTime closedDate)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        ClosedDate = closedDate;
    }
}