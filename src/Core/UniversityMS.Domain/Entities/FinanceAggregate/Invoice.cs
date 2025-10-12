using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FinanceAggregate;

/// <summary>
/// Fatura (Invoice) Entity
/// </summary>
public class Invoice : AuditableEntity
{
    public string InvoiceNumber { get; private set; } = null!;
    public InvoiceType Type { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public Guid? SupplierId { get; private set; }
    public Guid? CustomerId { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public Money TaxAmount { get; private set; } = null!;
    public Money NetAmount { get; private set; } = null!;
    public InvoiceStatus Status { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public string? Notes { get; private set; }

    // Collections
    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    private Invoice() { }

    private Invoice(
        string invoiceNumber,
        InvoiceType type,
        DateTime invoiceDate,
        DateTime dueDate,
        Guid? supplierId = null,
        Guid? customerId = null)
    {
        InvoiceNumber = invoiceNumber;
        Type = type;
        InvoiceDate = invoiceDate;
        DueDate = dueDate;
        SupplierId = supplierId;
        CustomerId = customerId;
        Status = InvoiceStatus.Draft;
        TotalAmount = Money.Zero();
        TaxAmount = Money.Zero();
        NetAmount = Money.Zero();
    }

    public static Invoice Create(
        string invoiceNumber,
        InvoiceType type,
        DateTime invoiceDate,
        DateTime dueDate,
        Guid? supplierId = null,
        Guid? customerId = null)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new DomainException("Fatura numarası boş olamaz.");

        if (dueDate < invoiceDate)
            throw new DomainException("Vade tarihi fatura tarihinden önce olamaz.");

        return new Invoice(invoiceNumber, type, invoiceDate, dueDate, supplierId, customerId);
    }

    public void AddItem(InvoiceItem item)
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Sadece taslak faturaya kalem eklenebilir.");

        _items.Add(item);
        RecalculateTotals();
    }

    public void Issue()
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("Sadece taslak fatura kesilebilir.");

        if (!_items.Any())
            throw new DomainException("Faturada en az bir kalem olmalıdır.");

        Status = InvoiceStatus.Issued;
    }

    public void MarkAsPaid(DateTime paymentDate)
    {
        if (Status != InvoiceStatus.Issued)
            throw new DomainException("Sadece kesilmiş fatura ödenebilir.");

        Status = InvoiceStatus.Paid;
        PaymentDate = paymentDate;
    }

    public void Cancel()
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Ödenmiş fatura iptal edilemez.");

        Status = InvoiceStatus.Cancelled;
    }

    private void RecalculateTotals()
    {
        var total = _items.Sum(i => i.TotalAmount.Amount);
        var tax = _items.Sum(i => i.TaxAmount.Amount);

        TotalAmount = Money.Create(total, "TRY");
        TaxAmount = Money.Create(tax, "TRY");
        NetAmount = Money.Create(total - tax, "TRY");
    }

    public bool IsOverdue()
    {
        return Status == InvoiceStatus.Issued && DateTime.UtcNow.Date > DueDate.Date;
    }
}