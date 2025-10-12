using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FinanceAggregate;

/// <summary>
/// Fatura Kalemi Entity
/// </summary>
public class InvoiceItem : AuditableEntity
{
    public Guid InvoiceId { get; private set; }
    public string Description { get; private set; } = null!;
    public decimal Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    public decimal TaxRate { get; private set; }
    public Money TaxAmount { get; private set; } = null!;
    public Money TotalAmount { get; private set; } = null!;

    public Invoice Invoice { get; private set; } = null!;

    private InvoiceItem() { }

    private InvoiceItem(
        Guid invoiceId,
        string description,
        decimal quantity,
        Money unitPrice,
        decimal taxRate)
    {
        InvoiceId = invoiceId;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TaxRate = taxRate;

        var subtotal = quantity * unitPrice.Amount;
        TaxAmount = Money.Create(subtotal * (taxRate / 100), unitPrice.Currency);
        TotalAmount = Money.Create(subtotal + TaxAmount.Amount, unitPrice.Currency);
    }

    public static InvoiceItem Create(
        Guid invoiceId,
        string description,
        decimal quantity,
        Money unitPrice,
        decimal taxRate = 20) // KDV %20 default
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");

        if (quantity <= 0)
            throw new DomainException("Miktar pozitif olmalıdır.");

        if (taxRate < 0 || taxRate > 100)
            throw new DomainException("Vergi oranı 0-100 arasında olmalıdır.");

        return new InvoiceItem(invoiceId, description, quantity, unitPrice, taxRate);
    }
}