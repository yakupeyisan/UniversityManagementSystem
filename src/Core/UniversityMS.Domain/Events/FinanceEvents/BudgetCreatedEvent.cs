using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.FinanceEvents;

public class BudgetCreatedEvent : BaseDomainEvent
{
    public Guid BudgetId { get; }
    public string BudgetCode { get; }
    public int FiscalYear { get; }
    public Money TotalAmount { get; }

    public BudgetCreatedEvent(Guid budgetId, string budgetCode, int fiscalYear, Money totalAmount)
    {
        BudgetId = budgetId;
        BudgetCode = budgetCode;
        FiscalYear = fiscalYear;
        TotalAmount = totalAmount;
    }
}

public class BudgetApprovedEvent : BaseDomainEvent
{
    public Guid BudgetId { get; }
    public int FiscalYear { get; }
    public Money TotalAmount { get; }

    public BudgetApprovedEvent(Guid budgetId, int fiscalYear, Money totalAmount)
    {
        BudgetId = budgetId;
        FiscalYear = fiscalYear;
        TotalAmount = totalAmount;
    }
}

public class BudgetAllocatedEvent : BaseDomainEvent
{
    public Guid BudgetId { get; }
    public Guid BudgetItemId { get; }
    public Money Amount { get; }

    public BudgetAllocatedEvent(Guid budgetId, Guid budgetItemId, Money amount)
    {
        BudgetId = budgetId;
        BudgetItemId = budgetItemId;
        Amount = amount;
    }
}

public class BudgetExpenseRecordedEvent : BaseDomainEvent
{
    public Guid BudgetId { get; }
    public Guid BudgetItemId { get; }
    public Money Amount { get; }

    public BudgetExpenseRecordedEvent(Guid budgetId, Guid budgetItemId, Money amount)
    {
        BudgetId = budgetId;
        BudgetItemId = budgetItemId;
        Amount = amount;
    }
}

public class TransactionCreatedEvent : BaseDomainEvent
{
    public Guid TransactionId { get; }
    public string TransactionNumber { get; }
    public TransactionType Type { get; }
    public Money Amount { get; }

    public TransactionCreatedEvent(Guid transactionId, string transactionNumber, TransactionType type, Money amount)
    {
        TransactionId = transactionId;
        TransactionNumber = transactionNumber;
        Type = type;
        Amount = amount;
    }
}

public class TransactionCompletedEvent : BaseDomainEvent
{
    public Guid TransactionId { get; }
    public string ReferenceNumber { get; }

    public TransactionCompletedEvent(Guid transactionId, string referenceNumber)
    {
        TransactionId = transactionId;
        ReferenceNumber = referenceNumber;
    }
}

public class InvoiceIssuedEvent : BaseDomainEvent
{
    public Guid InvoiceId { get; }
    public string InvoiceNumber { get; }
    public Money TotalAmount { get; }

    public InvoiceIssuedEvent(Guid invoiceId, string invoiceNumber, Money totalAmount)
    {
        InvoiceId = invoiceId;
        InvoiceNumber = invoiceNumber;
        TotalAmount = totalAmount;
    }
}

public class InvoicePaidEvent : BaseDomainEvent
{
    public Guid InvoiceId { get; }
    public DateTime PaymentDate { get; }
    public Money Amount { get; }

    public InvoicePaidEvent(Guid invoiceId, DateTime paymentDate, Money amount)
    {
        InvoiceId = invoiceId;
        PaymentDate = paymentDate;
        Amount = amount;
    }
}