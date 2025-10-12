using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.FinanceEvents;

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