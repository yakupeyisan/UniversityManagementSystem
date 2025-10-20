using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// IT destek talebi oluşturulduğunda tetiklenen event
/// </summary>
public class ITTicketCreatedEvent : BaseDomainEvent
{
    public Guid TicketId { get; }
    public string TicketNumber { get; }
    public Guid ReportedBy { get; }
    public ITTicketCategory Category { get; }
    public ITTicketSeverity Severity { get; }

    public ITTicketCreatedEvent(Guid ticketId, string ticketNumber, Guid reportedBy, ITTicketCategory category, ITTicketSeverity severity)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        ReportedBy = reportedBy;
        Category = category;
        Severity = severity;
    }
}