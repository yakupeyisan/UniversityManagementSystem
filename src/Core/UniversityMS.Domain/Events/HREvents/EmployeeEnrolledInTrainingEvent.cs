using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeEnrolledInTrainingEvent : BaseDomainEvent
{
    public Guid EmployeeId { get; }
    public Guid TrainingId { get; }
    public string TrainingTitle { get; }

    public EmployeeEnrolledInTrainingEvent(Guid employeeId, Guid trainingId, string trainingTitle)
    {
        EmployeeId = employeeId;
        TrainingId = trainingId;
        TrainingTitle = trainingTitle;
    }
}