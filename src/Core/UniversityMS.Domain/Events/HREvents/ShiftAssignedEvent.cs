using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class ShiftAssignedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid ShiftId { get; }
    public DateOnly Date { get; }
    public ShiftPattern Pattern { get; }

    public ShiftAssignedEvent(Guid employeeId, Guid shiftId, DateOnly date, ShiftPattern pattern)
    {
        EmployeeId = employeeId;
        ShiftId = shiftId;
        Date = date;
        Pattern = pattern;
    }
}