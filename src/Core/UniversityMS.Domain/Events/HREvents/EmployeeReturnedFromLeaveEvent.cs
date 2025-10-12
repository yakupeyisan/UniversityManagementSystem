using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeReturnedFromLeaveEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }

    public EmployeeReturnedFromLeaveEvent(Guid employeeId)
    {
        EmployeeId = employeeId;
    }
}