using System.Diagnostics;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.EnrollmentAggregate;


public class Enrollment : AuditableEntity, ISoftDelete
{
    public Guid StudentId { get; private set; }
    public string AcademicYear { get; private set; } // 2024-2025
    public int Semester { get; private set; } // 1 veya 2 (Güz/Bahar)
    public EnrollmentStatus Status { get; private set; }
    public DateTime EnrollmentDate { get; private set; }
    public DateTime? ApprovalDate { get; private set; }
    public Guid? ApprovedBy { get; private set; } // Danışman
    public int TotalECTS { get; private set; }
    public int TotalNationalCredit { get; private set; }

    // ISoftDelete implementation
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Student Student { get; private set; } = null!; // YENİ: Student navigation

    private readonly List<CourseRegistration> _courseRegistrations = new();
    public IReadOnlyCollection<CourseRegistration> CourseRegistrations => _courseRegistrations.AsReadOnly();

    private Enrollment() { } // EF Core

    private Enrollment(Guid studentId, string academicYear, int semester)
        : base()
    {
        StudentId = studentId;
        AcademicYear = academicYear;
        Semester = semester;
        Status = EnrollmentStatus.Draft;
        EnrollmentDate = DateTime.UtcNow;
        TotalECTS = 0;
        TotalNationalCredit = 0;
        IsDeleted = false;
    }

    public static Enrollment Create(Guid studentId, string academicYear, int semester)
    {
        if (studentId == Guid.Empty)
            throw new DomainException("Öğrenci ID geçersiz.");

        if (string.IsNullOrWhiteSpace(academicYear))
            throw new DomainException("Akademik yıl boş olamaz.");

        if (semester != 1 && semester != 2)
            throw new DomainException("Dönem 1 (Güz) veya 2 (Bahar) olmalıdır.");

        return new Enrollment(studentId, academicYear, semester);
    }

    public void AddCourse(Guid courseId, Course course)
    {
        if (_courseRegistrations.Any(cr => cr.CourseId == courseId))
            throw new DomainException("Bu ders zaten kayıtlı.");

        if (Status == EnrollmentStatus.Approved)
            throw new DomainException("Onaylanmış kayda ders eklenemez.");

        var registration = CourseRegistration.Create(Id, courseId, course.ECTS, course.NationalCredit);
        _courseRegistrations.Add(registration);

        RecalculateCredits();
    }

    public void RemoveCourse(Guid courseId)
    {
        if (Status == EnrollmentStatus.Approved)
            throw new DomainException("Onaylanmış kayıttan ders çıkarılamaz.");

        var registration = _courseRegistrations.FirstOrDefault(cr => cr.CourseId == courseId);
        if (registration != null)
        {
            _courseRegistrations.Remove(registration);
            RecalculateCredits();
        }
    }

    private void RecalculateCredits()
    {
        TotalECTS = _courseRegistrations.Sum(cr => cr.ECTS);
        TotalNationalCredit = _courseRegistrations.Sum(cr => cr.NationalCredit);
    }

    public void Submit()
    {
        if (Status != EnrollmentStatus.Draft)
            throw new DomainException("Sadece taslak durumundaki kayıt gönderilebilir.");

        if (!_courseRegistrations.Any())
            throw new DomainException("En az bir ders seçilmelidir.");

        Status = EnrollmentStatus.Submitted;
    }

    public void Approve(Guid advisorId)
    {
        if (Status != EnrollmentStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş kayıtlar onaylanabilir.");

        Status = EnrollmentStatus.Approved;
        ApprovalDate = DateTime.UtcNow;
        ApprovedBy = advisorId;
    }

    public void Reject(Guid advisorId, string reason)
    {
        if (Status != EnrollmentStatus.Submitted)
            throw new DomainException("Sadece gönderilmiş kayıtlar reddedilebilir.");

        Status = EnrollmentStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status == EnrollmentStatus.Approved)
            throw new DomainException("Onaylanmış kayıt iptal edilemez.");

        Status = EnrollmentStatus.Cancelled;
    }

    public void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    public bool CanModify() => Status == EnrollmentStatus.Draft || Status == EnrollmentStatus.Rejected;

    public void Restore()
    {
        IsDeleted = false;
        DeletedBy = null;
        DeletedAt = null;
    }
}

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
public class Grade : AuditableEntity
{
    public Guid CourseRegistrationId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid? InstructorId { get; private set; }
    public GradeType GradeType { get; private set; }
    public double NumericScore { get; private set; }
    public string LetterGrade { get; private set; }
    public double GradePoint { get; private set; }
    public double Weight { get; private set; } // Ağırlık (örn: 0.30 = %30)
    public DateTime GradeDate { get; private set; }
    public string? Notes { get; private set; }

    // Navigation Properties
    public CourseRegistration CourseRegistration { get; private set; } = null!;

    private Grade() { } // EF Core

    private Grade(Guid courseRegistrationId, Guid studentId, Guid courseId,
        GradeType gradeType, double numericScore, double weight, Guid? instructorId = null)
        : base()
    {
        CourseRegistrationId = courseRegistrationId;
        StudentId = studentId;
        CourseId = courseId;
        GradeType = gradeType;
        NumericScore = numericScore;
        Weight = weight;
        InstructorId = instructorId;
        GradeDate = DateTime.UtcNow;

        var gradeScore = GradeScore.Create(numericScore);
        LetterGrade = gradeScore.LetterGrade;
        GradePoint = gradeScore.GradePoint;
    }

    public static Grade Create(Guid courseRegistrationId, Guid studentId, Guid courseId,
        GradeType gradeType, double numericScore, double weight, Guid? instructorId = null)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new DomainException("Not 0-100 arasında olmalıdır.");

        if (weight < 0 || weight > 1)
            throw new DomainException("Ağırlık 0-1 arasında olmalıdır.");

        var grade = new Grade(courseRegistrationId, studentId, courseId, gradeType, numericScore, weight, instructorId);

        // Domain Event
        grade.AddDomainEvent(new GradeSubmittedEvent(studentId, courseId, numericScore, grade.LetterGrade, grade.GradePoint));

        return grade;
    }

    public void Update(double numericScore, string? notes = null)
    {
        if (numericScore < 0 || numericScore > 100)
            throw new DomainException("Not 0-100 arasında olmalıdır.");

        NumericScore = numericScore;
        Notes = notes;

        var gradeScore = GradeScore.Create(numericScore);
        LetterGrade = gradeScore.LetterGrade;
        GradePoint = gradeScore.GradePoint;
    }

    public double GetWeightedScore()
    {
        return NumericScore * Weight;
    }

    public double GetWeightedGradePoint()
    {
        return GradePoint * Weight;
    }
}


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

public class GradeObjection : AuditableEntity
{
    public Guid GradeId { get; private set; }
    public Guid StudentId { get; private set; }
    public Guid CourseId { get; private set; }
    public string Reason { get; private set; }
    public ObjectionStatus Status { get; private set; }
    public DateTime ObjectionDate { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewDate { get; private set; }
    public string? ReviewNotes { get; private set; }
    public double? OldScore { get; private set; }
    public double? NewScore { get; private set; }

    // Navigation Properties
    public Grade Grade { get; private set; } = null!;

    private GradeObjection() { } // EF Core

    private GradeObjection(Guid gradeId, Guid studentId, Guid courseId, string reason)
        : base()
    {
        GradeId = gradeId;
        StudentId = studentId;
        CourseId = courseId;
        Reason = reason;
        Status = ObjectionStatus.Pending;
        ObjectionDate = DateTime.UtcNow;
    }

    public static GradeObjection Create(Guid gradeId, Guid studentId, Guid courseId, string reason)
    {
        if (gradeId == Guid.Empty)
            throw new DomainException("Not ID geçersiz.");

        if (studentId == Guid.Empty)
            throw new DomainException("Öğrenci ID geçersiz.");

        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("İtiraz nedeni belirtilmelidir.");

        return new GradeObjection(gradeId, studentId, courseId, reason);
    }

    public void Approve(Guid reviewedBy, double newScore, string? notes = null)
    {
        if (Status != ObjectionStatus.Pending)
            throw new DomainException("Sadece beklemedeki itirazlar onaylanabilir.");

        if (newScore < 0 || newScore > 100)
            throw new DomainException("Yeni not 0-100 arasında olmalıdır.");

        Status = ObjectionStatus.Approved;
        ReviewedBy = reviewedBy;
        ReviewDate = DateTime.UtcNow;
        ReviewNotes = notes;
        NewScore = newScore;
    }

    public void Reject(Guid reviewedBy, string? notes = null)
    {
        if (Status != ObjectionStatus.Pending)
            throw new DomainException("Sadece beklemedeki itirazlar reddedilebilir.");

        Status = ObjectionStatus.Rejected;
        ReviewedBy = reviewedBy;
        ReviewDate = DateTime.UtcNow;
        ReviewNotes = notes;
    }

    public void SetUnderReview()
    {
        if (Status != ObjectionStatus.Pending)
            throw new DomainException("Sadece beklemedeki itirazlar incelemeye alınabilir.");

        Status = ObjectionStatus.UnderReview;
    }
}