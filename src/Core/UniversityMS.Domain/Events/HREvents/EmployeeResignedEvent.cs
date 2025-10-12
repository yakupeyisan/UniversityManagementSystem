using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeResignedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public DateTime ResignationDate { get; }

    public EmployeeResignedEvent(Guid employeeId, DateTime resignationDate)
    {
        EmployeeId = employeeId;
        ResignationDate = resignationDate;
    }
}