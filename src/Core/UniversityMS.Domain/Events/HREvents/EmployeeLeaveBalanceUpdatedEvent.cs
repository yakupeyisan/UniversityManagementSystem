using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeLeaveBalanceUpdatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public int NewBalance { get; }

    public EmployeeLeaveBalanceUpdatedEvent(Guid employeeId, int newBalance)
    {
        EmployeeId = employeeId;
        NewBalance = newBalance;
    }
}