using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeAnnualLeaveRefreshedEvent(Guid employeeId) : BaseDomainEvent
{
    public Guid EmployeeId { get; } = employeeId;
}