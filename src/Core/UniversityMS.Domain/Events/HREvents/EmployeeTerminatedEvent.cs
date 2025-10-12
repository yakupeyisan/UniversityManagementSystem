using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeTerminatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public DateTime TerminationDate { get; }
    public string Reason { get; }

    public EmployeeTerminatedEvent(Guid employeeId, DateTime terminationDate, string reason)
    {
        EmployeeId = employeeId;
        TerminationDate = terminationDate;
        Reason = reason;
    }
}