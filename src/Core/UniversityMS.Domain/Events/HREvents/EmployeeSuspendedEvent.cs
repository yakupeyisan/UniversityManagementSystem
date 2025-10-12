using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeSuspendedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public string Reason { get; }

    public EmployeeSuspendedEvent(Guid employeeId, string reason)
    {
        EmployeeId = employeeId;
        Reason = reason;
    }
}