using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Events.ScheduleEvents;
using UniversityMS.Domain.Exceptions;

namespace UniversityMS.Domain.Entities.ScheduleAggregate;


/// <summary>
/// Ders Programı (Aggregate Root)
/// Bir dönem için genel ders programını temsil eder
/// </summary>
public class Schedule : AuditableEntity, ISoftDelete
{
    public string AcademicYear { get; private set; } // 2024-2025
    public int Semester { get; private set; } // 1 veya 2
    public Guid? DepartmentId { get; private set; } // Null ise genel program
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public ScheduleStatus Status { get; private set; }
    public DateTime? PublishedDate { get; private set; }
    public Guid? PublishedBy { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation - Course Sessions
    private readonly List<CourseSession> _courseSessions = new();
    public IReadOnlyCollection<CourseSession> CourseSessions => _courseSessions.AsReadOnly();

    private Schedule() { } // EF Core

    private Schedule(string academicYear, int semester, string name, DateTime startDate, DateTime endDate)
    {
        AcademicYear = academicYear;
        Semester = semester;
        Name = name;
        Status = ScheduleStatus.Draft;
        StartDate = startDate;
        EndDate = endDate;
        IsDeleted = false;
    }

    public static Schedule Create(string academicYear, int semester, string name,
        DateTime startDate, DateTime endDate, Guid? departmentId = null)
    {
        if (string.IsNullOrWhiteSpace(academicYear))
            throw new DomainException("Akademik yıl boş olamaz.");

        if (semester != 1 && semester != 2)
            throw new DomainException("Dönem 1 (Güz) veya 2 (Bahar) olmalıdır.");

        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Program adı boş olamaz.");

        if (startDate >= endDate)
            throw new DomainException("Bitiş tarihi başlangıç tarihinden sonra olmalıdır.");

        var schedule = new Schedule(academicYear, semester, name, startDate, endDate)
        {
            DepartmentId = departmentId
        };

        return schedule;
    }

    public void AddCourseSession(Guid courseId, Guid? instructorId, Guid classroomId,
        DayOfWeekEnum dayOfWeek, TimeSpan startTime, TimeSpan endTime, SessionType sessionType)
    {
        if (Status == ScheduleStatus.Archived)
            throw new DomainException("Arşivlenmiş programa ders eklenemez.");

        var session = CourseSession.Create(
            Id, courseId, instructorId, classroomId,
            dayOfWeek, startTime, endTime, sessionType
        );

        // Çakışma kontrolü
        if (HasConflict(session))
            throw new DomainException("Bu zaman diliminde çakışma var.");

        _courseSessions.Add(session);
    }

    public void RemoveCourseSession(Guid sessionId)
    {
        if (Status == ScheduleStatus.Active)
            throw new DomainException("Aktif programdan ders çıkarılamaz. Önce askıya alın.");

        var session = _courseSessions.FirstOrDefault(s => s.Id == sessionId);
        if (session != null)
        {
            _courseSessions.Remove(session);
        }
    }

    public bool HasConflict(CourseSession newSession)
    {
        // Aynı gün, aynı derslik, çakışan saat kontrolü
        var conflictingSession = _courseSessions.FirstOrDefault(s =>
            s.DayOfWeek == newSession.DayOfWeek &&
            s.ClassroomId == newSession.ClassroomId &&
            s.TimeSlot.ConflictsWith(newSession.TimeSlot) &&
            !s.IsDeleted
        );

        if (conflictingSession != null)
            return true;

        // Aynı öğretim üyesi çakışması
        if (newSession.InstructorId.HasValue)
        {
            var instructorConflict = _courseSessions.FirstOrDefault(s =>
                s.InstructorId == newSession.InstructorId &&
                s.DayOfWeek == newSession.DayOfWeek &&
                s.TimeSlot.ConflictsWith(newSession.TimeSlot) &&
                !s.IsDeleted
            );

            if (instructorConflict != null)
                return true;
        }

        return false;
    }

    public void Publish(Guid publishedBy)
    {
        if (Status != ScheduleStatus.Draft)
            throw new DomainException("Sadece taslak programlar yayınlanabilir.");

        if (!_courseSessions.Any())
            throw new DomainException("Boş program yayınlanamaz.");

        Status = ScheduleStatus.Published;
        PublishedDate = DateTime.UtcNow;
        PublishedBy = publishedBy;

        AddDomainEvent(new SchedulePublishedEvent(Id, AcademicYear, Semester));
    }

    public void Activate()
    {
        if (Status != ScheduleStatus.Published)
            throw new DomainException("Sadece yayınlanmış programlar aktif hale getirilebilir.");

        Status = ScheduleStatus.Active;
    }

    public void Suspend()
    {
        if (Status != ScheduleStatus.Active)
            throw new DomainException("Sadece aktif programlar askıya alınabilir.");

        Status = ScheduleStatus.Suspended;
    }

    public void Archive()
    {
        if (Status == ScheduleStatus.Draft)
            throw new DomainException("Taslak program arşivlenemez.");

        Status = ScheduleStatus.Archived;
    }

    public void Delete(string? deletedBy = null)
    {
        if (Status == ScheduleStatus.Active)
            throw new DomainException("Aktif program silinemez. Önce askıya alın.");

        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }

    public int GetTotalSessionCount() => _courseSessions.Count(s => !s.IsDeleted);

    public int GetInstructorWorkload(Guid instructorId)
    {
        return _courseSessions
            .Where(s => s.InstructorId == instructorId && !s.IsDeleted)
            .Sum(s => s.TimeSlot.DurationMinutes / 50); // 50 dakikalık ders saati
    }
}