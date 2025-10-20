using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.DocumentEvents;

/// <summary>
/// Belge imzalandığında tetiklenen event
/// </summary>
public class DocumentSignedEvent : BaseDomainEvent
{
    public Guid DocumentId { get; }
    public Guid SignedBy { get; }
    public DateTime SignatureDate { get; }

    public DocumentSignedEvent(Guid documentId, Guid signedBy, DateTime signatureDate)
    {
        DocumentId = documentId;
        SignedBy = signedBy;
        SignatureDate = signatureDate;
    }
}