using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// IT talebi çözüldüğünde tetiklenen event
/// </summary>
public class ITTicketResolvedEvent : BaseDomainEvent
{
    public Guid TicketId { get; }
    public string TicketNumber { get; }
    public Guid ResolvedBy { get; }
    public DateTime ResolvedDate { get; }
    public string ResolutionSummary { get; }

    public ITTicketResolvedEvent(Guid ticketId, string ticketNumber, Guid resolvedBy, DateTime resolvedDate, string resolutionSummary)
    {
        TicketId = ticketId;
        TicketNumber = ticketNumber;
        ResolvedBy = resolvedBy;
        ResolvedDate = resolvedDate;
        ResolutionSummary = resolutionSummary;
    }
}