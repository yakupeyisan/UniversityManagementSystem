using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.EnrollmentAggregate;

public class CourseRegistration : AuditableEntity, ISoftDelete
{
    public Guid EnrollmentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid? InstructorId { get; private set; }
    public int ECTS { get; private set; }
    public int NationalCredit { get; private set; }
    public CourseRegistrationStatus Status { get; private set; }
    public DateTime RegistrationDate { get; private set; }
    public DateTime? DropDate { get; private set; }

    // ISoftDelete implementation
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Enrollment Enrollment { get; private set; } = null!;
    public Course Course { get; private set; } = null!;

    private readonly List<Grade> _grades = new();
    public IReadOnlyCollection<Grade> Grades => _grades.AsReadOnly();

    private readonly List<Attendance> _attendances = new();
    public IReadOnlyCollection<Attendance> Attendances => _attendances.AsReadOnly();

    private CourseRegistration() { } // EF Core

    private CourseRegistration(Guid enrollmentId, Guid courseId, int ects, int nationalCredit)
        : base()
    {
        EnrollmentId = enrollmentId;
        CourseId = courseId;
        ECTS = ects;
        NationalCredit = nationalCredit;
        Status = CourseRegistrationStatus.Active;
        RegistrationDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static CourseRegistration Create(Guid enrollmentId, Guid courseId, int ects, int nationalCredit)
    {
        return new CourseRegistration(enrollmentId, courseId, ects, nationalCredit);
    }

    public void AssignInstructor(Guid instructorId)
    {
        InstructorId = instructorId;
    }

    public void Drop()
    {
        if (Status != CourseRegistrationStatus.Active)
            throw new DomainException("Sadece aktif kayıtlar bırakılabilir.");

        Status = CourseRegistrationStatus.Dropped;
        DropDate = DateTime.UtcNow;
    }

    public void Complete(string finalLetterGrade, double gradePoint)
    {
        if (Status != CourseRegistrationStatus.Active)
            throw new DomainException("Sadece aktif kayıtlar tamamlanabilir.");

        Status = gradePoint >= 2.0 ? CourseRegistrationStatus.Passed : CourseRegistrationStatus.Failed;
    }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public double? GetFinalGrade()
    {
        var finalGrade = _grades.FirstOrDefault(g => g.GradeType == GradeType.Final);
        return finalGrade?.NumericScore;
    }

    public double GetAttendanceRate()
    {
        if (!_attendances.Any())
            return 100.0;

        var presentCount = _attendances.Count(a => a.IsPresent);
        return (double)presentCount / _attendances.Count * 100;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}