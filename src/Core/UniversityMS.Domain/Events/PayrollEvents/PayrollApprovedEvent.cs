using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.PayrollEvents;

public class PayrollApprovedEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }
    public Guid ApproverId { get; }

    public PayrollApprovedEvent(Guid payrollId, Guid employeeId, Guid approverId)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
        ApproverId = approverId;
    }
}