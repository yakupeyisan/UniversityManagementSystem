using MediatR;
using Microsoft.Extensions.Logging;
using UniversityMS.Application.Common.Models;
using UniversityMS.Application.Features.ScheduleFeature.DTOs;
using UniversityMS.Domain.Entities.ScheduleAggregate;
using UniversityMS.Domain.Interfaces;

namespace UniversityMS.Application.Features.ScheduleFeature.Commands;

public class CheckScheduleConflictsCommandHandler : IRequestHandler<CheckScheduleConflictsCommand, Result<List<ScheduleConflictDto>>>
{
    private readonly IRepository<CourseSession> _courseSessionRepository;
    private readonly ILogger<CheckScheduleConflictsCommandHandler> _logger;

    public CheckScheduleConflictsCommandHandler(
        IRepository<CourseSession> courseSessionRepository,
        ILogger<CheckScheduleConflictsCommandHandler> logger)
    {
        _courseSessionRepository = courseSessionRepository;
        _logger = logger;
    }

    public async Task<Result<List<ScheduleConflictDto>>> Handle(
        CheckScheduleConflictsCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var sessions = await _courseSessionRepository.FindAsync(
                cs => cs.ScheduleId == request.ScheduleId,
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

            _logger.LogInformation("Schedule conflict check completed. ConflictCount: {ConflictCount}", conflicts.Count);
            return Result<List<ScheduleConflictDto>>.Success(conflicts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking schedule conflicts");
            return Result<List<ScheduleConflictDto>>.Failure("Çakışma kontrolü sırasında bir hata oluştu.", ex.Message);
        }
    }

    private bool HasConflict(CourseSession session1, CourseSession session2)
    {
        // Aynı gün olmalı
        if (session1.DayOfWeek != session2.DayOfWeek)
            return false;

        // Aynı hafta olmalı
        if (session1.WeekNumber != session2.WeekNumber)
            return false;

        // Öğretim üyesi çakışması
        if (session1.InstructorId == session2.InstructorId)
            return TimeConflict(session1, session2);

        // Sınıf çakışması
        if (session1.ClassroomId == session2.ClassroomId)
            return TimeConflict(session1, session2);

        return false;
    }

    private bool TimeConflict(CourseSession session1, CourseSession session2)
    {
        return !(session1.EndTime <= session2.StartTime || session2.EndTime <= session1.StartTime);
    }

    private string DetermineConflictType(CourseSession session1, CourseSession session2)
    {
        if (session1.InstructorId == session2.InstructorId)
            return "Instructor";
        if (session1.ClassroomId == session2.ClassroomId)
            return "Classroom";
        return "Unknown";
    }
}