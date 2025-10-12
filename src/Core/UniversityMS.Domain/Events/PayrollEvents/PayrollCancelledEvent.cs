using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.PayrollEvents;

public class PayrollCancelledEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }

    public PayrollCancelledEvent(Guid payrollId, Guid employeeId)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
    }
}