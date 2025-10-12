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