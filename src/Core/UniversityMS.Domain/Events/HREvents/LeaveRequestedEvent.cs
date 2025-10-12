using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class LeaveRequestedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid LeaveId { get; }
    public LeaveType LeaveType { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }

    public LeaveRequestedEvent(Guid employeeId, Guid leaveId, LeaveType leaveType, DateTime startDate, DateTime endDate)
    {
        EmployeeId = employeeId;
        LeaveId = leaveId;
        LeaveType = leaveType;
        StartDate = startDate;
        EndDate = endDate;
    }
}