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
/// 
public class PurchaseRequest : AuditableEntity, ISoftDelete
{
    public string RequestNumber { get; private set; } // Auto-generated
    public Guid DepartmentId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public ProcurementStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime RequiredDate { get; private set; }
    public string Priority { get; private set; } // Low, Medium, High
    public Guid RequestedBy { get; private set; } // Staff ID
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public string? RejectionReason { get; private set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    // Navigation
    private readonly List<PurchaseItem> _items = new();
    public IReadOnlyCollection<PurchaseItem> Items => _items.AsReadOnly();

    private PurchaseRequest() { }

    private PurchaseRequest(string requestNumber, Guid departmentId, string title,
        string description, DateTime requiredDate, string priority, Guid requestedBy)
    {
        RequestNumber = requestNumber;
        DepartmentId = departmentId;
        Title = title;
        Description = description;
        Status = ProcurementStatus.Draft;
        RequiredDate = requiredDate;
        Priority = priority;
        RequestedBy = requestedBy;
        IsDeleted = false;
    }

    public static PurchaseRequest Create(string requestNumber, Guid departmentId,
        string title, string description, DateTime requiredDate, string priority, Guid requestedBy)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("Başlık boş olamaz.");
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");
        if (requiredDate <= DateTime.UtcNow)
            throw new DomainException("Gerekli tarih gelecek bir tarih olmalıdır.");
        if (!new[] { "Low", "Medium", "High" }.Contains(priority))
            throw new DomainException("Geçersiz öncelik.");

        return new PurchaseRequest(requestNumber, departmentId, title, description,
            requiredDate, priority, requestedBy);
    }

    public void AddItem(Guid itemId, string itemName, string description,
        int quantity, decimal unitPrice, string unit)
    {
        if (Status != ProcurementStatus.Draft)
            throw new DomainException("Taslak durumunda olmayan taleplere ürün eklenemez.");

        var item = PurchaseItem.Create(Id, itemId, itemName, description, quantity, unitPrice, unit);
        _items.Add(item);
        RecalculateTotal();
    }

    public void RemoveItem(Guid itemId)
    {
        if (Status != ProcurementStatus.Draft)
            throw new DomainException("Taslak durumunda olmayan taleplerden ürün çıkarılamaz.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            RecalculateTotal();
        }
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.Quantity * i.UnitPrice);
    }

    public void Submit()
    {
        if (Status != ProcurementStatus.Draft)
            throw new DomainException("Sadece taslak talepleri gönderilebilir.");
        if (!_items.Any())
            throw new DomainException("En az bir ürün eklenmelidir.");

        Status = ProcurementStatus.Submitted;
    }

    public void Approve(Guid approvedBy)
    {
        if (Status != ProcurementStatus.Submitted)
            throw new DomainException("Sadece gönderilen talepler onaylanabilir.");

        Status = ProcurementStatus.Approved;
        ApprovedBy = approvedBy;
        ApprovalDate = DateTime.UtcNow;
    }

    public void Reject(string reason)
    {
        if (Status != ProcurementStatus.Submitted)
            throw new DomainException("Sadece gönderilen talepler reddedilebilir.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Reddetme nedeni boş olamaz.");

        Status = ProcurementStatus.Rejected;
        RejectionReason = reason;
    }

    public void CreateOrder()
    {
        if (Status != ProcurementStatus.Approved)
            throw new DomainException("Sadece onaylanan talepler sipariş edilebilir.");

        Status = ProcurementStatus.Ordered;
    }

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

}