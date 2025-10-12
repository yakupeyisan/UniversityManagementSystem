using UniversityMS.Domain.Interfaces;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Events.PayrollEvents;

public class PayrollPaidEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }
    public Money NetSalary { get; }
    public DateTime PaymentDate { get; }

    public PayrollPaidEvent(Guid payrollId, Guid employeeId, Money netSalary, DateTime paymentDate)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
        NetSalary = netSalary;
        PaymentDate = paymentDate;
    }
}