using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.DocumentManagementAggregate;

/// <summary>
/// Belge Geçmişi
/// </summary>
public class DocumentHistory : AuditableEntity
{
    public Guid DocumentId { get; private set; }
    public string Action { get; private set; } = null!;
    public DocumentHistoryAction ActionType { get; private set; }
    public Guid PerformedBy { get; private set; }
    public string? Details { get; private set; }

    private DocumentHistory() { }

    private DocumentHistory(Guid docId, string action, DocumentHistoryAction actionType, Guid performedBy)
    {
        DocumentId = docId;
        Action = action;
        ActionType = actionType;
        PerformedBy = performedBy;
    }

    public static DocumentHistory Create(Guid docId, string action, DocumentHistoryAction actionType, Guid performedBy)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new DomainException("İşlem açıklaması boş olamaz.");
        return new DocumentHistory(docId, action, actionType, performedBy);
    }
}