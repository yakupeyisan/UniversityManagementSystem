using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeTransferredEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid? OldDepartmentId { get; }
    public Guid NewDepartmentId { get; }

    public EmployeeTransferredEvent(Guid employeeId, Guid? oldDepartmentId, Guid newDepartmentId)
    {
        EmployeeId = employeeId;
        OldDepartmentId = oldDepartmentId;
        NewDepartmentId = newDepartmentId;
    }
}