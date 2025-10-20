using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Entities.ITManagementAggregate;

/// <summary>
/// IT Destek Talebi - Aggregate Root
/// </summary>
public class ITTicket : AuditableEntity, IAggregateRoot, ISoftDelete
{
    public string TicketNumber { get; private set; } = null!;
    public Guid ReportedBy { get; private set; }
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ITTicketCategory Category { get; private set; }
    public ITTicketSeverity Severity { get; private set; }
    public ITTicketStatus Status { get; private set; }
    public ITTicketPriority Priority { get; private set; }
    public DateTime ReportedDate { get; private set; }
    public DateTime? AssignedDate { get; private set; }
    public DateTime? ResolvedDate { get; private set; }
    public Guid? AssignedTo { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public int EstimatedResolutionHours { get; private set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
    private readonly List<ITTicketComment> _comments = new();
    public IReadOnlyCollection<ITTicketComment> Comments => _comments.AsReadOnly();

    public Person ReportedByUser { get; private set; } = null!;

    private ITTicket() { }

    private ITTicket(string ticketNo, Guid reportedBy, string title, string description, ITTicketCategory category, ITTicketSeverity severity)
    {
        TicketNumber = ticketNo;
        ReportedBy = reportedBy;
        Title = title;
        Description = description;
        Category = category;
        Severity = severity;
        Status = ITTicketStatus.New;
        Priority = severity == ITTicketSeverity.Critical ? ITTicketPriority.Urgent : ITTicketPriority.Normal;
        ReportedDate = DateTime.UtcNow;
        EstimatedResolutionHours = 24;
        IsDeleted = false;
    }

    public static ITTicket Create(string ticketNo, Guid reportedBy, string title, string description, ITTicketCategory category, ITTicketSeverity severity)
    {
        if (string.IsNullOrWhiteSpace(ticketNo) || string.IsNullOrWhiteSpace(title))
            throw new DomainException("Talep numarası ve başlık boş olamaz.");
        return new ITTicket(ticketNo, reportedBy, title, description, category, severity);
    }

    public void Assign(Guid assignedTo, int estimatedHours)
    {
        if (Status == ITTicketStatus.Resolved)
            throw new DomainException("Çözülmüş talep atanmaz.");
        AssignedTo = assignedTo;
        AssignedDate = DateTime.UtcNow;
        Status = ITTicketStatus.Assigned;
        EstimatedResolutionHours = estimatedHours;
    }

    public void StartWork()
    {
        if (Status != ITTicketStatus.Assigned)
            throw new DomainException("Sadece atanmış talepler üzerinde çalışılabilir.");
        Status = ITTicketStatus.InProgress;
    }

    public void Resolve(string resolutionNotes)
    {
        if (Status != ITTicketStatus.InProgress)
            throw new DomainException("Sadece devam eden talepler çözülebilir.");
        Status = ITTicketStatus.Resolved;
        ResolutionNotes = resolutionNotes;
        ResolvedDate = DateTime.UtcNow;
    }

    public void AddComment(ITTicketComment comment) => _comments.Add(comment);
}