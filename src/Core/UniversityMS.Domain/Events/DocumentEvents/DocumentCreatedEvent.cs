using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.DocumentEvents;


/// <summary>
/// Belge oluşturulduğunda tetiklenen event
/// </summary>
public class DocumentCreatedEvent : BaseDomainEvent
{
    public Guid DocumentId { get; }
    public string DocumentNumber { get; }
    public DocumentType Type { get; }
    public Guid UserId { get; }

    public DocumentCreatedEvent(Guid documentId, string documentNumber, DocumentType type, Guid userId)
    {
        DocumentId = documentId;
        DocumentNumber = documentNumber;
        Type = type;
        UserId = userId;
    }
}