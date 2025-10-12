using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FinanceAggregate;

/// <summary>
/// Mali İşlem (Transaction) Entity
/// Tüm gelir-gider hareketlerini kaydeder
/// </summary>
public class Transaction : AuditableEntity
{
    public string TransactionNumber { get; private set; } = null!;
    public TransactionType Type { get; private set; }
    public DateTime TransactionDate { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public Guid? PayerId { get; private set; }
    public Guid? PayeeId { get; private set; }
    public Guid? BudgetItemId { get; private set; }
    public Guid? InvoiceId { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public string? ReferenceNumber { get; private set; }
    public TransactionStatus Status { get; private set; }
    public string? Notes { get; private set; }

    private Transaction() { }

    private Transaction(
        string transactionNumber,
        TransactionType type,
        DateTime transactionDate,
        Money amount,
        string description,
        PaymentMethod paymentMethod,
        Guid? payerId = null,
        Guid? payeeId = null,
        Guid? budgetItemId = null,
        Guid? invoiceId = null)
    {
        TransactionNumber = transactionNumber;
        Type = type;
        TransactionDate = transactionDate;
        Amount = amount;
        Description = description;
        PaymentMethod = paymentMethod;
        PayerId = payerId;
        PayeeId = payeeId;
        BudgetItemId = budgetItemId;
        InvoiceId = invoiceId;
        Status = TransactionStatus.Pending;
    }

    public static Transaction Create(
        string transactionNumber,
        TransactionType type,
        DateTime transactionDate,
        Money amount,
        string description,
        PaymentMethod paymentMethod,
        Guid? payerId = null,
        Guid? payeeId = null,
        Guid? budgetItemId = null,
        Guid? invoiceId = null)
    {
        if (string.IsNullOrWhiteSpace(transactionNumber))
            throw new DomainException("İşlem numarası boş olamaz.");

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Açıklama boş olamaz.");

        return new Transaction(
            transactionNumber, type, transactionDate, amount, description,
            paymentMethod, payerId, payeeId, budgetItemId, invoiceId);
    }

    public void Complete(string referenceNumber)
    {
        if (Status != TransactionStatus.Pending)
            throw new DomainException("Sadece bekleyen işlemler tamamlanabilir.");

        Status = TransactionStatus.Completed;
        ReferenceNumber = referenceNumber;
    }

    public void Cancel(string reason)
    {
        if (Status == TransactionStatus.Completed)
            throw new DomainException("Tamamlanmış işlem iptal edilemez.");

        Status = TransactionStatus.Cancelled;
        Notes = reason;
    }
}