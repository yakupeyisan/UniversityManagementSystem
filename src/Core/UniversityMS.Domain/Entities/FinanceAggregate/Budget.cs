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