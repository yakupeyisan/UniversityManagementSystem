using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.PayrollEvents;
public class PayrollCalculatedEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }
    public PayrollPeriod Period { get; }
    public Money NetSalary { get; }

    public PayrollCalculatedEvent(Guid payrollId, Guid employeeId, PayrollPeriod period, Money netSalary)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
        Period = period;
        NetSalary = netSalary;
    }
}