using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class PayslipGeneratedEvent(Guid payrollId, Guid payslipId, Guid employeeId) : BaseDomainEvent
{
    public Guid PayrollId { get; } = payrollId;
    public Guid PayslipId { get; } = payslipId;
    public Guid EmployeeId { get; } = employeeId;
}