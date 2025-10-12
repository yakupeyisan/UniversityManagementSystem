using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeReactivatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }

    public EmployeeReactivatedEvent(Guid employeeId)
    {
        EmployeeId = employeeId;
    }
}