using UniversityMS.Application.Common.Interfaces;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Enums;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Infrastructure.Services;

public class ScheduleConflictService : IScheduleConflictService
{
    private readonly IRepository<CourseSession> _courseSessionRepository;

    public ScheduleConflictService(IRepository<CourseSession> courseSessionRepository)
    {
        _courseSessionRepository = courseSessionRepository;
    }

    public async Task<List<ScheduleConflictDto>> CheckConflictsAsync(Guid scheduleId, CancellationToken cancellationToken)
    {
        var sessions = await _courseSessionRepository.FindAsync(
            cs => cs.ScheduleId == scheduleId,
            cancellationToken);

        var conflicts = new List<ScheduleConflictDto>();

        for (int i = 0; i < sessions.Count; i++)
        {
            for (int j = i + 1; j < sessions.Count; j++)
            {
                var session1 = sessions[i];
                var session2 = sessions[j];

                if (HasConflict(session1, session2))
                {
                    conflicts.Add(new ScheduleConflictDto
                    {
                        Session1Id = session1.Id,
                        Session2Id = session2.Id,
                        ConflictType = DetermineConflictType(session1, session2),
                        Message = $"Çakışma: {session1.Course.Code} ve {session2.Course.Code}"
                    });
                }
            }
        }

        return conflicts;
    }

    public async Task<bool> HasInstructorConflictAsync(
        Guid instructorId,
        DayOfWeek day,
        TimeSpan start,
        TimeSpan end,
        CancellationToken cancellationToken)
    {
        // DayOfWeek enum'ını DayOfWeekEnum'e dönüştür
        var dayOfWeekEnum = (DayOfWeekEnum)(int)day;

        var sessions = await _courseSessionRepository.FindAsync(
            cs => cs.InstructorId == instructorId && cs.DayOfWeek == dayOfWeekEnum,
            cancellationToken);

        return sessions.Any(s => TimeConflict(start, end, s.TimeSlot.StartTime, s.TimeSlot.EndTime));
    }

    public async Task<bool> HasClassroomConflictAsync(
        Guid classroomId,
        DayOfWeek day,
        TimeSpan start,
        TimeSpan end,
        CancellationToken cancellationToken)
    {
        // DayOfWeek enum'ını DayOfWeekEnum'e dönüştür
        var dayOfWeekEnum = (DayOfWeekEnum)(int)day;

        var sessions = await _courseSessionRepository.FindAsync(
            cs => cs.ClassroomId == classroomId && cs.DayOfWeek == dayOfWeekEnum,
            cancellationToken);

        return sessions.Any(s => TimeConflict(start, end, s.TimeSlot.StartTime, s.TimeSlot.EndTime));
    }

    private bool HasConflict(CourseSession session1, CourseSession session2)
    {
        // Aynı gün kontrol
        if (session1.DayOfWeek != session2.DayOfWeek)
            return false;

        // Öğretim üyesi çakışması
        if (session1.InstructorId.HasValue && session2.InstructorId.HasValue &&
            session1.InstructorId == session2.InstructorId)
        {
            return TimeConflict(
                session1.TimeSlot.StartTime, session1.TimeSlot.EndTime,
                session2.TimeSlot.StartTime, session2.TimeSlot.EndTime);
        }

        // Sınıf çakışması
        if (session1.ClassroomId == session2.ClassroomId)
        {
            return TimeConflict(
                session1.TimeSlot.StartTime, session1.TimeSlot.EndTime,
                session2.TimeSlot.StartTime, session2.TimeSlot.EndTime);
        }

        return false;
    }

    private bool TimeConflict(TimeSpan start1, TimeSpan end1, TimeSpan start2, TimeSpan end2)
    {
        return !(end1 <= start2 || end2 <= start1);
    }

    private string DetermineConflictType(CourseSession session1, CourseSession session2)
    {
        if (session1.InstructorId.HasValue && session2.InstructorId.HasValue &&
            session1.InstructorId == session2.InstructorId)
            return "Instructor";
        if (session1.ClassroomId == session2.ClassroomId)
            return "Classroom";
        return "Unknown";
    }
}