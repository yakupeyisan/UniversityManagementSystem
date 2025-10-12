using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class LeaveRejectedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }
    public Guid RejectorId { get; }
    public string Reason { get; }

    public LeaveRejectedEvent(Guid employeeId, Guid leaveId, Guid rejectorId, string reason)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
        RejectorId = rejectorId;
        Reason = reason;
    }
}