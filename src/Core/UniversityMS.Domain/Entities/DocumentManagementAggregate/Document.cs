using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.DocumentManagementAggregate;

/// <summary>
/// Belge - Aggregate Root
/// </summary>
public class Document : AuditableEntity, IAggregateRoot, ISoftDelete
{
    public string DocumentNumber { get; private set; } = null!;
    public DocumentType Type { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime IssueDate { get; private set; }
    public DateTime? ExpiryDate { get; private set; }
    public DocumentStatus Status { get; private set; }
    public string? FilePath { get; private set; }
    public string? FileName { get; private set; }
    public long? FileSize { get; private set; }
    public string? FileHash { get; private set; }
    public bool IsSigned { get; private set; }
    public Guid? SignedBy { get; private set; }
    public DateTime? SignedDate { get; private set; }
    public int CopyCount { get; private set; }
    public string? Notes { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public void Delete(string? deletedBy = null)
    {
        IsDeleted= true;
        DeletedAt= DateTime.UtcNow;
        DeletedBy= deletedBy;
    }

    public void Restore()
    {
        IsDeleted= false;
        DeletedAt= null;
        DeletedBy= null;
    }

    private readonly List<DocumentHistory> _history = new();
    public IReadOnlyCollection<DocumentHistory> History => _history.AsReadOnly();

    public Person User { get; private set; } = null!;

    private Document() { }

    private Document(string docNumber, DocumentType type, Guid userId, DateTime issueDate)
    {
        DocumentNumber = docNumber;
        Type = type;
        UserId = userId;
        IssueDate = issueDate;
        Status = DocumentStatus.Draft;
        IsSigned = false;
        CopyCount = 1;
        IsDeleted = false;
    }

    public static Document Create(string docNumber, DocumentType type, Guid userId, DateTime issueDate)
    {
        if (string.IsNullOrWhiteSpace(docNumber))
            throw new DomainException("Belge numarası boş olamaz.");

        return new Document(docNumber, type, userId, issueDate);
    }

    public void Publish()
    {
        if (Status != DocumentStatus.Draft)
            throw new DomainException("Sadece taslak belgeler yayınlanabilir.");
        Status = DocumentStatus.Published;
    }

    public void Sign(Guid signedBy)
    {
        IsSigned = true;
        SignedBy = signedBy;
        SignedDate = DateTime.UtcNow;
        var history = DocumentHistory.Create(Id, "Belge imzalandı", DocumentHistoryAction.Signed, signedBy);
        _history.Add(history);
    }

    public void AttachFile(string filePath, string fileName, long fileSize, string fileHash)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new DomainException("Dosya yolu boş olamaz.");
        FilePath = filePath;
        FileName = fileName;
        FileSize = fileSize;
        FileHash = fileHash;
    }

    public void AddHistory(DocumentHistory history) => _history.Add(history);
}