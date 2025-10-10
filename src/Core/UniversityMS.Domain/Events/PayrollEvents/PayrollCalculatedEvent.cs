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

public class PayrollApprovedEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }
    public Guid ApproverId { get; }

    public PayrollApprovedEvent(Guid payrollId, Guid employeeId, Guid approverId)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
        ApproverId = approverId;
    }
}

public class PayrollRejectedEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }
    public string Reason { get; }

    public PayrollRejectedEvent(Guid payrollId, Guid employeeId, string reason)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
        Reason = reason;
    }
}

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

public class PayrollCancelledEvent : BaseDomainEvent
{
    public Guid PayrollId { get; }
    public Guid EmployeeId { get; }

    public PayrollCancelledEvent(Guid payrollId, Guid employeeId)
    {
        PayrollId = payrollId;
        EmployeeId = employeeId;
    }
}