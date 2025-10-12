using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.FinanceEvents;

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