using UniversityMS.Domain.Entities.AcademicAggregate;
using UniversityMS.Domain.Entities.Common;
using UniversityMS.Domain.Entities.FacilityAggregate;
using UniversityMS.Domain.Entities.PersonAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Exceptions;
using UniversityMS.Domain.ValueObjects;

namespace UniversityMS.Domain.Entities.ScheduleAggregate;

/// <summary>
/// Ders Oturumu - Programdaki tek bir ders saati
/// </summary>
public class CourseSession : AuditableEntity, ISoftDelete
{
    public Guid ScheduleId { get; private set; }
    public Guid CourseId { get; private set; }
    public Guid? InstructorId { get; private set; } // Null olabilir (henüz atanmamış)
    public Guid ClassroomId { get; private set; }
    public DayOfWeekEnum DayOfWeek { get; private set; }
    public TimeSlot TimeSlot { get; private set; }
    public SessionType SessionType { get; private set; }
    public string? Notes { get; private set; }

    // ISoftDelete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public Schedule Schedule { get; private set; } = null!;
    public Course Course { get; private set; } = null!;
    public Classroom Classroom { get; private set; } = null!;
    public Staff? Instructor { get; private set; }

    private CourseSession() { } // EF Core

    private CourseSession(Guid scheduleId, Guid courseId, Guid? instructorId, Guid classroomId,
        DayOfWeekEnum dayOfWeek, TimeSlot timeSlot, SessionType sessionType)
    {
        ScheduleId = scheduleId;
        CourseId = courseId;
        InstructorId = instructorId;
        ClassroomId = classroomId;
        DayOfWeek = dayOfWeek;
        TimeSlot = timeSlot;
        SessionType = sessionType;
        IsDeleted = false;
    }

    public static CourseSession Create(Guid scheduleId, Guid courseId, Guid? instructorId,
        Guid classroomId, DayOfWeekEnum dayOfWeek, TimeSpan startTime, TimeSpan endTime,
        SessionType sessionType)
    {
        if (scheduleId == Guid.Empty)
            throw new DomainException("Program ID geçersiz.");

        if (courseId == Guid.Empty)
            throw new DomainException("Ders ID geçersiz.");

        if (classroomId == Guid.Empty)
            throw new DomainException("Derslik ID geçersiz.");

        var timeSlot = TimeSlot.Create(startTime, endTime);

        return new CourseSession(scheduleId, courseId, instructorId, classroomId,
            dayOfWeek, timeSlot, sessionType);
    }

    public void AssignInstructor(Guid instructorId)
    {
        if (instructorId == Guid.Empty)
            throw new DomainException("Öğretim üyesi ID geçersiz.");

        InstructorId = instructorId;
    }

    public void RemoveInstructor()
    {
        InstructorId = null;
    }

    public void ChangeClassroom(Guid newClassroomId)
    {
        if (newClassroomId == Guid.Empty)
            throw new DomainException("Derslik ID geçersiz.");

        ClassroomId = newClassroomId;
    }

    public void ChangeTime(DayOfWeekEnum newDayOfWeek, TimeSpan newStartTime, TimeSpan newEndTime)
    {
        DayOfWeek = newDayOfWeek;
        TimeSlot = TimeSlot.Create(newStartTime, newEndTime);
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
    }

    public void Delete(string? deletedBy = null)
    {
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

    public string GetDayName()
    {
        return DayOfWeek switch
        {
            DayOfWeekEnum.Monday => "Pazartesi",
            DayOfWeekEnum.Tuesday => "Salı",
            DayOfWeekEnum.Wednesday => "Çarşamba",
            DayOfWeekEnum.Thursday => "Perşembe",
            DayOfWeekEnum.Friday => "Cuma",
            DayOfWeekEnum.Saturday => "Cumartesi",
            DayOfWeekEnum.Sunday => "Pazar",
            _ => "Bilinmeyen"
        };
    }
}