using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.PayrollEvents;

public class PayrollRejectedEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }
    public string Reason { get; }

    public PayrollRejectedEvent(Guid payrollId, Guid employeeId, string reason)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
        Reason = reason;
    }
}