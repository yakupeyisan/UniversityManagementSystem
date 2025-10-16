using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events.HREvents;

public class EmployeeAcademicTitleUpdatedEvent(
    Guid employeeId,
    AcademicTitle? oldAcademicTitle,
    AcademicTitle newAcademicTitle)
    : BaseDomainEvent
{
    public Guid EmployeeId { get; } = employeeId;
    public AcademicTitle? OldAcademicTitle { get; } = oldAcademicTitle;
    public AcademicTitle NewAcademicTitle { get; } = newAcademicTitle;
}