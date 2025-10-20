using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// IT talebi yeniden açıldığında tetiklenen event
/// </summary>
public class ITTicketReopenedEvent : BaseDomainEvent
{
    public Guid TicketId { get; }
    public string TicketNumber { get; }
    public string ReopenReason { get; }

    public ITTicketReopenedEvent(Guid ticketId, string ticketNumber, string reopenReason)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        ReopenReason = reopenReason;
    }
}