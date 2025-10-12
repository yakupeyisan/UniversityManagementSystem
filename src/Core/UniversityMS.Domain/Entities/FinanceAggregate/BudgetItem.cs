using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.FinanceAggregate;

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