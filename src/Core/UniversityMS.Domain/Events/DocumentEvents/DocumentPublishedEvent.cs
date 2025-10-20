using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.DocumentEvents;

/// <summary>
/// Belge yayınlandığında tetiklenen event
/// </summary>
public class DocumentPublishedEvent : BaseDomainEvent
{
    public Guid DocumentId { get; }
    public string DocumentNumber { get; }
    public DateTime PublishDate { get; }

    public DocumentPublishedEvent(Guid documentId, string documentNumber, DateTime publishDate)
    {
        DocumentId = documentId;
        DocumentNumber = documentNumber;
        PublishDate = publishDate;
    }
}