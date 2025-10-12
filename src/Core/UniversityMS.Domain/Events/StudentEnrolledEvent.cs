using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Domain.Events;

public class StudentEnrolledEvent : BaseDomainEvent
{
    public Guid StudentId { get; }
    public string StudentNumber { get; }
    public Guid DepartmentId { get; }

    public StudentEnrolledEvent(Guid studentId, string studentNumber, Guid departmentId)
    {
        StudentId = studentId;
        StudentNumber = studentNumber;
        DepartmentId = departmentId;
    }
}