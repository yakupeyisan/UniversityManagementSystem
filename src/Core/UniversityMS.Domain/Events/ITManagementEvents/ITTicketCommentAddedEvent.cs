using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.ITManagementEvents;

/// <summary>
/// IT talebi yorum ekleme
/// </summary>
public class ITTicketCommentAddedEvent : BaseDomainEvent
{
    public Guid TicketId { get; }
    public Guid CommentId { get; }
    public Guid CommentedBy { get; }
    public string Comment { get; }
    public bool IsInternalNote { get; }

    public ITTicketCommentAddedEvent(Guid ticketId, Guid commentId, Guid commentedBy, string comment, bool isInternalNote)
    {
        TicketId = ticketId;
        CommentId = commentId;
        CommentedBy = commentedBy;
        Comment = comment;
        IsInternalNote = isInternalNote;
    }
}