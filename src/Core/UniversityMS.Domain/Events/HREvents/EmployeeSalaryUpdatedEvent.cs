using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeSalaryUpdatedEvent(Guid employeeId, decimal oldSalary, decimal newSalary) : BaseDomainEvent
{
    public Guid EmployeeId { get; } = employeeId;
    public decimal OldSalary { get; } = oldSalary;
    public decimal NewSalary { get; } = newSalary;
}