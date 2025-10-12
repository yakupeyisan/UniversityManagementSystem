using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.ProcurementEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.ProcurementAggregate;

/// <summary>
/// Satın Alma Siparişi (PurchaseOrder) - Aggregate Root
/// </summary>
public class PurchaseOrder : AuditableEntity, IAggregateRoot
{
    public string OrderNumber { get; private set; } = null!;
    public Guid SupplierId { get; private set; }
    public Guid? PurchaseRequestId { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime ExpectedDeliveryDate { get; private set; }
    public PurchaseOrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public string? DeliveryAddress { get; private set; }
    public string? Terms { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public string? Notes { get; private set; }

    // Collections
    private readonly List<PurchaseOrderItem> _items = new();
    public IReadOnlyCollection<PurchaseOrderItem> Items => _items.AsReadOnly();

    private PurchaseOrder() { }

    private PurchaseOrder(
        string orderNumber,
        Guid supplierId,
        DateTime orderDate,
        DateTime expectedDeliveryDate,
        string? deliveryAddress = null,
        Guid? purchaseRequestId = null)
    {
        OrderNumber = orderNumber;
        SupplierId = supplierId;
        PurchaseRequestId = purchaseRequestId;
        OrderDate = orderDate;
        ExpectedDeliveryDate = expectedDeliveryDate;
        Status = PurchaseOrderStatus.Draft;
        TotalAmount = Money.Zero();
        DeliveryAddress = deliveryAddress;
    }

    public static PurchaseOrder Create(
        string orderNumber,
        Guid supplierId,
        DateTime orderDate,
        DateTime expectedDeliveryDate,
        string? deliveryAddress = null,
        Guid? purchaseRequestId = null)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainException("Sipariş numarası boş olamaz.");

        if (expectedDeliveryDate < orderDate)
            throw new DomainException("Teslimat tarihi sipariş tarihinden önce olamaz.");

        return new PurchaseOrder(orderNumber, supplierId, orderDate, expectedDeliveryDate, deliveryAddress, purchaseRequestId);
    }

    public void AddItem(PurchaseOrderItem item)
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Sadece taslak siparişlere kalem eklenebilir.");

        _items.Add(item);
        RecalculateTotal();
    }

    public void Submit()
    {
        if (Status != PurchaseOrderStatus.Draft)
            throw new DomainException("Sadece taslak siparişler gönderilebilir.");

        Status = PurchaseOrderStatus.Submitted;
    }

    public void Approve(Guid approverId)
    {
        if (Status != PurchaseOrderStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş siparişler onaylanabilir.");

        Status = PurchaseOrderStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;

        AddDomainEvent(new PurchaseOrderApprovedEvent(Id, OrderNumber, SupplierId));
    }

    public void MarkAsReceived()
    {
        if (Status != PurchaseOrderStatus.Approved)
            throw new DomainException("Sadece onaylı siparişler teslim alınabilir.");

        Status = PurchaseOrderStatus.Received;
        AddDomainEvent(new PurchaseOrderReceivedEvent(Id, OrderNumber));
    }

    public void Complete()
    {
        if (Status != PurchaseOrderStatus.Received)
            throw new DomainException("Sipariş teslim alınmadan tamamlanamaz.");

        Status = PurchaseOrderStatus.Completed;
    }

    private void RecalculateTotal()
    {
        TotalAmount = Money.Create(_items.Sum(i => i.TotalAmount.Amount), "TRY");
    }
}