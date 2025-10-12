using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.ProcurementAggregate;

/// <summary>
/// Satın Alma Talebi Kalemi Entity
/// </summary>
public class PurchaseRequestItem : AuditableEntity
{
    public Guid PurchaseRequestId { get; private set; }
    public string ItemName { get; private set; } = null!;
    public string? Description { get; private set; }
    public string? Specifications { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = null!;
    public Money EstimatedUnitPrice { get; private set; } = null!;
    public Money EstimatedTotal { get; private set; } = null!;

    public PurchaseRequest PurchaseRequest { get; private set; } = null!;

    private PurchaseRequestItem() { }

    private PurchaseRequestItem(
        Guid purchaseRequestId,
        string itemName,
        decimal quantity,
        string unit,
        Money estimatedUnitPrice,
        string? description = null,
        string? specifications = null)
    {
        PurchaseRequestId = purchaseRequestId;
        ItemName = itemName;
        Quantity = quantity;
        Unit = unit;
        EstimatedUnitPrice = estimatedUnitPrice;
        EstimatedTotal = Money.Create(quantity * estimatedUnitPrice.Amount, estimatedUnitPrice.Currency);
        Description = description;
        Specifications = specifications;
    }

    public static PurchaseRequestItem Create(
        Guid purchaseRequestId,
        string itemName,
        decimal quantity,
        string unit,
        Money estimatedUnitPrice,
        string? description = null,
        string? specifications = null)
    {
        if (string.IsNullOrWhiteSpace(itemName))
            throw new DomainException("Malzeme adı boş olamaz.");

        if (quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        if (string.IsNullOrWhiteSpace(unit))
            throw new DomainException("Birim belirtilmelidir.");

        return new PurchaseRequestItem(purchaseRequestId, itemName, quantity, unit, estimatedUnitPrice, description, specifications);
    }
}