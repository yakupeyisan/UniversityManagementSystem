using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.CafeteriaAggregate;

/// <summary>
/// Yemekhane Sipariş Kalemi
/// </summary>
public class CafeteriaOrderItem : AuditableEntity
{
    public Guid CafeteriaOrderId { get; private set; }
    public Guid MenuItemId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public Money UnitPrice { get; private set; } = null!;
    public int Quantity { get; private set; }
    public Money TotalAmount { get; private set; } = null!;

    public CafeteriaOrder CafeteriaOrder { get; private set; } = null!;

    private CafeteriaOrderItem() { }

    private CafeteriaOrderItem(Guid orderId, Guid menuItemId, string itemName, Money unitPrice, int qty)
    {
        CafeteriaOrderId = orderId;
        MenuItemId = menuItemId;
        ItemName = itemName;
        UnitPrice = unitPrice;
        Quantity = qty;
        TotalAmount = Money.Create(qty * unitPrice.Amount, unitPrice.Currency);
    }

    public static CafeteriaOrderItem Create(Guid orderId, Guid menuItemId, string itemName, Money unitPrice, int qty)
    {
        if (qty <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        return new CafeteriaOrderItem(orderId, menuItemId, itemName, unitPrice, qty);
    }
}