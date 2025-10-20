using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.CafeteriaEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.CafeteriaAggregate;

/// <summary>
/// Yemekhane Siparişi - Aggregate Root
/// </summary>
public class CafeteriaOrder : AuditableEntity, IAggregateRoot
{
    public string OrderNumber { get; private set; } = null!;
    public Guid UserId { get; private set; }
    public CafeteriaOrderStatus Status { get; private set; }
    public DateTime OrderDate { get; private set; }
    public DateTime? DeliveryDate { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public Money DiscountAmount { get; private set; } = null!;
    public Money NetAmount { get; private set; } = null!;
    public PaymentStatus PaymentStatus { get; private set; }
    public DateTime? PaidDate { get; private set; }

    private readonly List<CafeteriaOrderItem> _items = new();
    public IReadOnlyCollection<CafeteriaOrderItem> Items => _items.AsReadOnly();

    public Person User { get; private set; } = null!;

    private CafeteriaOrder() { }

    private CafeteriaOrder(string orderNumber, Guid userId)
    {
        OrderNumber = orderNumber;
        UserId = userId;
        Status = CafeteriaOrderStatus.Pending;
        OrderDate = DateTime.UtcNow;
        TotalAmount = Money.Zero();
        DiscountAmount = Money.Zero();
        NetAmount = Money.Zero();
        PaymentStatus = PaymentStatus.Pending;
    }

    public static CafeteriaOrder Create(string orderNumber, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(orderNumber))
            throw new DomainException("Sipariş numarası boş olamaz.");

        var order = new CafeteriaOrder(orderNumber, userId);
        order.AddDomainEvent(new CafeteriaOrderCreatedEvent(order.Id, orderNumber, userId));
        return order;
    }

    public void AddItem(CafeteriaOrderItem item)
    {
        _items.Add(item);
        RecalculateTotals();
    }

    public void RemoveItem(Guid itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item != null) { _items.Remove(item); RecalculateTotals(); }
    }

    public void Approve() => Status = CafeteriaOrderStatus.Approved;

    public void MarkAsDelivered()
    {
        Status = CafeteriaOrderStatus.Delivered;
        DeliveryDate = DateTime.UtcNow;
    }

    public void MarkAsPaid()
    {
        PaymentStatus = PaymentStatus.Completed;
        PaidDate = DateTime.UtcNow;
    }

    public void ApplyDiscount(Money discountAmount)
    {
        if (discountAmount.Amount < 0 || discountAmount.Amount > TotalAmount.Amount)
            throw new DomainException("Geçersiz indirim.");
        DiscountAmount = discountAmount;
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        var total = _items.Sum(x => x.TotalAmount.Amount);
        TotalAmount = Money.Create(total, "TRY");
        NetAmount = Money.Create(total - DiscountAmount.Amount, "TRY");
    }
}