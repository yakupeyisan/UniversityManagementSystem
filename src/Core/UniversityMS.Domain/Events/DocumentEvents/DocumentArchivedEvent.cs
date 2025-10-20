using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.DocumentEvents;

/// <summary>
/// Belge arşivlendiğinde tetiklenen event
/// </summary>
public class DocumentArchivedEvent : BaseDomainEvent
{
    public Guid DocumentId { get; }
    public DateTime ArchiveDate { get; }

    public DocumentArchivedEvent(Guid documentId, DateTime archiveDate)
    {
        DocumentId = documentId;
        ArchiveDate = archiveDate;
    }
}