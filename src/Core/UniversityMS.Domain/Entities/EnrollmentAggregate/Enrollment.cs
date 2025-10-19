using System.Diagnostics;
using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.EnrollmentAggregate;


public class Enrollment : AuditableEntity, ISoftDelete
{
    private const int MAX_ECTS_PER_SEMESTER = 30; // Türkiye standardı
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

        var newTotalECTS = TotalECTS + course.ECTS;
        if (newTotalECTS > MAX_ECTS_PER_SEMESTER)
            throw new DomainException(
                $"ECTS limiti aşıldı. Maksimum: {MAX_ECTS_PER_SEMESTER}, " +
                $"Yeni toplam: {newTotalECTS}");

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