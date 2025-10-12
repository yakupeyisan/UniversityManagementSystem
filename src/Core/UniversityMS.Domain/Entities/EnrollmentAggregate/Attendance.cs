using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.EnrollmentAggregate;

public class Attendance : AuditableEntity
{
    public Guid CourseRegistrationId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public DateTime AttendanceDate { get; private set; }
    public int WeekNumber { get; private set; }
    public bool IsPresent { get; private set; }
    public string? Notes { get; private set; }
    public AttendanceMethod Method { get; private set; }

    // Navigation Properties
    public CourseRegistration CourseRegistration { get; private set; } = null!;

    private Attendance() { } // EF Core

    private Attendance(Guid courseRegistrationId, Guid studentId, Guid courseId,
        DateTime attendanceDate, int weekNumber, bool isPresent, AttendanceMethod method)
        : base()
    {
        CourseRegistrationId = courseRegistrationId;
        StudentId = studentId;
        CourseId = courseId;
        AttendanceDate = attendanceDate;
        WeekNumber = weekNumber;
        IsPresent = isPresent;
        Method = method;
    }

    public static Attendance Create(Guid courseRegistrationId, Guid studentId, Guid courseId,
        DateTime attendanceDate, int weekNumber, bool isPresent,
        AttendanceMethod method = AttendanceMethod.Manual)
    {
        if (weekNumber < 1 || weekNumber > 16)
            throw new DomainException("Hafta numarası 1-16 arasında olmalıdır.");

        return new Attendance(courseRegistrationId, studentId, courseId,
            attendanceDate, weekNumber, isPresent, method);
    }

    public void MarkPresent()
    {
        IsPresent = true;
    }

    public void MarkAbsent()
    {
        IsPresent = false;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
    }
}