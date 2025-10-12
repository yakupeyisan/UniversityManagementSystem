using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class LeaveApprovedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }
    public Guid ApproverId { get; }

    public LeaveApprovedEvent(Guid employeeId, Guid leaveId, Guid approverId)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
        ApproverId = approverId;
    }
}