using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeRetiredEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public DateTime RetirementDate { get; }

    public EmployeeRetiredEvent(Guid employeeId, DateTime retirementDate)
    {
        EmployeeId = employeeId;
        RetirementDate = retirementDate;
    }
}