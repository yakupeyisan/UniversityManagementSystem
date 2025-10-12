using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.FinanceEvents;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FinanceAggregate;


/// <summary>
/// Bütçe (Budget) - Aggregate Root
/// Yıllık/Dönemsel bütçe planlaması ve takibi
/// </summary>
public class Budget : AuditableEntity, IAggregateRoot
{
    public string BudgetCode { get; private set; } = null!;
    public int FiscalYear { get; private set; }
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public BudgetType Type { get; private set; }
    public BudgetStatus Status { get; private set; }
    public Guid? DepartmentId { get; private set; }
    public Money TotalAmount { get; private set; } = null!;
    public Money AllocatedAmount { get; private set; } = null!;
    public Money SpentAmount { get; private set; } = null!;
    public Money RemainingAmount { get; private set; } = null!;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid? ApprovedBy { get; private set; }
    public DateTime? ApprovedDate { get; private set; }
    public string? Notes { get; private set; }

    // Collections
    private readonly List<BudgetItem> _items = new();
    public IReadOnlyCollection<BudgetItem> Items => _items.AsReadOnly();

    private Budget() { }

    private Budget(
        string budgetCode,
        int fiscalYear,
        string name,
        string description,
        BudgetType type,
        Money totalAmount,
        DateTime startDate,
        DateTime endDate,
        Guid? departmentId = null)
    {
        BudgetCode = budgetCode;
        FiscalYear = fiscalYear;
        Name = name;
        Description = description;
        Type = type;
        Status = BudgetStatus.Draft;
        TotalAmount = totalAmount;
        AllocatedAmount = Money.Zero();
        SpentAmount = Money.Zero();
        RemainingAmount = totalAmount;
        StartDate = startDate;
        EndDate = endDate;
        DepartmentId = departmentId;
    }

    public static Budget Create(
        string budgetCode,
        int fiscalYear,
        string name,
        string description,
        BudgetType type,
        Money totalAmount,
        DateTime startDate,
        DateTime endDate,
        Guid? departmentId = null)
    {
        if (string.IsNullOrWhiteSpace(budgetCode))
            throw new DomainException("Bütçe kodu boş olamaz.");

        if (fiscalYear < 2000 || fiscalYear > 2100)
            throw new DomainException("Geçerli bir mali yıl giriniz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Bütçe adı boş olamaz.");

        if (endDate <= startDate)
            throw new DomainException("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        return new Budget(budgetCode, fiscalYear, name, description, type, totalAmount, startDate, endDate, departmentId);
    }

    public void AddItem(BudgetItem item)
    {
        if (Status == BudgetStatus.Closed)
            throw new DomainException("Kapalı bütçeye kalem eklenemez.");

        _items.Add(item);
        RecalculateAmounts();
    }

    public void AllocateBudget(Guid itemId, Money amount)
    {
        if (Status != BudgetStatus.Approved)
            throw new DomainException("Sadece onaylı bütçelerden tahsisat yapılabilir.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException("Bütçe kalemi bulunamadı.");

        item.Allocate(amount);
        RecalculateAmounts();

        AddDomainEvent(new BudgetAllocatedEvent(Id, itemId, amount));
    }

    public void RecordExpense(Guid itemId, Money amount)
    {
        if (Status != BudgetStatus.Approved && Status != BudgetStatus.Active)
            throw new DomainException("Bütçe aktif değil.");

        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException("Bütçe kalemi bulunamadı.");

        item.RecordExpense(amount);
        RecalculateAmounts();

        AddDomainEvent(new BudgetExpenseRecordedEvent(Id, itemId, amount));
    }

    public void Approve(Guid approverId)
    {
        if (Status != BudgetStatus.Draft && Status != BudgetStatus.UnderReview)
            throw new DomainException("Bütçe onaylanabilir durumda değil.");

        Status = BudgetStatus.Approved;
        ApprovedBy = approverId;
        ApprovedDate = DateTime.UtcNow;

        AddDomainEvent(new BudgetApprovedEvent(Id, FiscalYear, TotalAmount));
    }

    public void Activate()
    {
        if (Status != BudgetStatus.Approved)
            throw new DomainException("Sadece onaylı bütçeler aktif edilebilir.");

        Status = BudgetStatus.Active;
    }

    public void Close()
    {
        if (Status != BudgetStatus.Active && Status != BudgetStatus.Approved)
            throw new DomainException("Bütçe kapatılabilir durumda değil.");

        Status = BudgetStatus.Closed;
    }

    private void RecalculateAmounts()
    {
        AllocatedAmount = Money.Create(_items.Sum(i => i.AllocatedAmount.Amount), TotalAmount.Currency);
        SpentAmount = Money.Create(_items.Sum(i => i.SpentAmount.Amount), TotalAmount.Currency);
        RemainingAmount = Money.Create(TotalAmount.Amount - SpentAmount.Amount, TotalAmount.Currency);
    }

    public decimal GetUtilizationRate()
    {
        if (TotalAmount.Amount == 0) return 0;
        return (SpentAmount.Amount / TotalAmount.Amount) * 100;
    }
}

/// <summary>
/// Bütçe Kalemi Entity
/// </summary>
public class BudgetItem : AuditableEntity
{
    public Guid BudgetId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public BudgetCategory Category { get; private set; }
    public Money PlannedAmount { get; private set; } = null!;
    public Money AllocatedAmount { get; private set; } = null!;
    public Money SpentAmount { get; private set; } = null!;

    public Budget Budget { get; private set; } = null!;

    private BudgetItem() { }

    private BudgetItem(
        Guid budgetId,
        string code,
        string name,
        BudgetCategory category,
        Money plannedAmount,
        string? description = null)
    {
        BudgetId = budgetId;
        Code = code;
        Name = name;
        Category = category;
        PlannedAmount = plannedAmount;
        AllocatedAmount = Money.Zero();
        SpentAmount = Money.Zero();
        Description = description;
    }

    public static BudgetItem Create(
        Guid budgetId,
        string code,
        string name,
        BudgetCategory category,
        Money plannedAmount,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new DomainException("Kod boş olamaz.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Ad boş olamaz.");

        return new BudgetItem(budgetId, code, name, category, plannedAmount, description);
    }

    public void Allocate(Money amount)
    {
        if (AllocatedAmount.Amount + amount.Amount > PlannedAmount.Amount)
            throw new DomainException("Tahsisat planlanandan fazla olamaz.");

        AllocatedAmount = Money.Create(AllocatedAmount.Amount + amount.Amount, amount.Currency);
    }

    public void RecordExpense(Money amount)
    {
        if (SpentAmount.Amount + amount.Amount > AllocatedAmount.Amount)
            throw new DomainException("Harcama tahsisattan fazla olamaz.");

        SpentAmount = Money.Create(SpentAmount.Amount + amount.Amount, amount.Currency);
    }

    public Money GetRemainingBudget()
    {
        return Money.Create(AllocatedAmount.Amount - SpentAmount.Amount, AllocatedAmount.Currency);
    }
}

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