using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.ProcurementEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.ProcurementAggregate;

/// <summary>
/// Satın Alma Talebi (PurchaseRequest) - Aggregate Root
/// </summary>
public class PurchaseRequest : AuditableEntity, IAggregateRoot
{
    public string RequestNumber { get; private set; } = null!;
    public Guid RequestorId { get; private set; }
    public Guid DepartmentId { get; private set; }
    public DateTime RequestDate { get; private set; }
    public DateTime RequiredDate { get; private set; }
    public PurchaseRequestStatus Status { get; private set; }
    public PurchasePriority Priority { get; private set; }
    public string Justification { get; private set; } = null!;
    public Money EstimatedTotal { get; private set; } = null!;
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? Notes { get; private set; }

    // Collections
    private readonly List<PurchaseRequestItem> _items = new();
    public IReadOnlyCollection<PurchaseRequestItem> Items => _items.AsReadOnly();

    private PurchaseRequest() { }

    private PurchaseRequest(
        string requestNumber,
        Guid requestorId,
        Guid departmentId,
        DateTime requestDate,
        DateTime requiredDate,
        PurchasePriority priority,
        string justification)
    {
        RequestNumber = requestNumber;
        RequestorId = requestorId;
        DepartmentId = departmentId;
        RequestDate = requestDate;
        RequiredDate = requiredDate;
        Priority = priority;
        Justification = justification;
        Status = PurchaseRequestStatus.Draft;
        EstimatedTotal = Money.Zero();
    }

    public static PurchaseRequest Create(
        string requestNumber,
        Guid requestorId,
        Guid departmentId,
        DateTime requestDate,
        DateTime requiredDate,
        PurchasePriority priority,
        string justification)
    {
        if (string.IsNullOrWhiteSpace(requestNumber))
            throw new DomainException("Talep numarası boş olamaz.");

        if (requiredDate < requestDate)
            throw new DomainException("İhtiyaç tarihi talep tarihinden önce olamaz.");

        if (string.IsNullOrWhiteSpace(justification))
            throw new DomainException("Gerekçe belirtilmelidir.");

        return new PurchaseRequest(requestNumber, requestorId, departmentId, requestDate, requiredDate, priority, justification);
    }

    public void AddItem(PurchaseRequestItem item)
    {
        if (Status != PurchaseRequestStatus.Draft)
            throw new DomainException("Sadece taslak taleplere kalem eklenebilir.");

        _items.Add(item);
        RecalculateTotal();
    }

    public void Submit()
    {
        if (Status != PurchaseRequestStatus.Draft)
            throw new DomainException("Sadece taslak talepler gönderilebilir.");

        if (!_items.Any())
            throw new DomainException("En az bir kalem eklenmelidir.");

        Status = PurchaseRequestStatus.Submitted;
        AddDomainEvent(new PurchaseRequestSubmittedEvent(Id, RequestNumber));
    }

    public void Approve(Guid approverId)
    {
        if (Status != PurchaseRequestStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş talepler onaylanabilir.");

        Status = PurchaseRequestStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;

        AddDomainEvent(new PurchaseRequestApprovedEvent(Id, approverId));
    }

    public void Reject(string reason)
    {
        if (Status != PurchaseRequestStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş talepler reddedilebilir.");

        Status = PurchaseRequestStatus.Rejected;
        RejectionReason = reason;
    }

    private void RecalculateTotal()
    {
        EstimatedTotal = Money.Create(_items.Sum(i => i.EstimatedTotal.Amount), "TRY");
    }
}