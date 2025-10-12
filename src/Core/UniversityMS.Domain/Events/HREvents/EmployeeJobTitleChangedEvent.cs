using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeJobTitleChangedEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public string OldTitle { get; }
    public string NewTitle { get; }

    public EmployeeJobTitleChangedEvent(Guid employeeId, string oldTitle, string newTitle)
    {
        EmployeeId = employeeId;
        OldTitle = oldTitle;
        NewTitle = newTitle;
    }
}