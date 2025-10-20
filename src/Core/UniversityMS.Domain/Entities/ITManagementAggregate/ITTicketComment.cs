using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.ITManagementAggregate;

/// <summary>
/// IT Talep Yorumu
/// </summary>
public class ITTicketComment : AuditableEntity
{
    public Guid ITTicketId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string Comment { get; private set; } = null!;
    public bool IsInternalNote { get; private set; }

    public ITTicket ITTicket { get; private set; } = null!;
    public Person CreatedByUser { get; private set; } = null!;

    private ITTicketComment() { }

    private ITTicketComment(Guid ticketId, Guid userId, string comment, bool isInternal = false)
    {
        ITTicketId = ticketId;
        CreatedByUserId = userId;
        Comment = comment;
        IsInternalNote = isInternal;
    }

    public static ITTicketComment Create(Guid ticketId, Guid userId, string comment, bool isInternal = false)
    {
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("Yorum boş olamaz.");
        return new ITTicketComment(ticketId, userId, comment, isInternal);
    }
}