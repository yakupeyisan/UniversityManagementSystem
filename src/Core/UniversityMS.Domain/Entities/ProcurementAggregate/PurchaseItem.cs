using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.ProcurementAggregate;

public class PurchaseItem : BaseEntity
{
    public Guid PurchaseRequestId { get; private set; }
    public Guid ItemId { get; private set; }
    public string ItemName { get; private set; }
    public string Description { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Unit { get; private set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    private PurchaseItem() { }

    private PurchaseItem(Guid purchaseRequestId, Guid itemId, string itemName,
        string description, int quantity, decimal unitPrice, string unit)
    {
        PurchaseRequestId = purchaseRequestId;
        ItemId = itemId;
        ItemName = itemName;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Unit = unit;
    }

    public static PurchaseItem Create(Guid purchaseRequestId, Guid itemId, string itemName,
        string description, int quantity, decimal unitPrice, string unit)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Ürün adı boş olamaz.");
        if (quantity <= 0)
            throw new DomainException("Miktar 0'dan büyük olmalıdır.");
        if (unitPrice <= 0)
            throw new DomainException("Birim fiyat 0'dan büyük olmalıdır.");

        return new PurchaseItem(purchaseRequestId, itemId, itemName, description, quantity, unitPrice, unit);
    }
}