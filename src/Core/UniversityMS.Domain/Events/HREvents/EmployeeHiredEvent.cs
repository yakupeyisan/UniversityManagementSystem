using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.HREvents;
public class EmployeeHiredEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public EmployeeNumber EmployeeNumber { get; }
    public Guid PersonId { get; }
    public DateTime HireDate { get; }

    public EmployeeHiredEvent(Guid employeeId, EmployeeNumber employeeNumber, Guid personId, DateTime hireDate)
    {
        EmployeeId = employeeId;
        EmployeeNumber = employeeNumber;
        PersonId = personId;
        HireDate = hireDate;
    }
}