using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeSalaryUpdatedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public decimal OldSalary { get; }
    public decimal NewSalary { get; }

    public EmployeeSalaryUpdatedEvent(Guid employeeId, decimal oldSalary, decimal newSalary)
    {
        EmployeeId = employeeId;
        OldSalary = oldSalary;
        NewSalary = newSalary;
    }
}